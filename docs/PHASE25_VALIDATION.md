# Phase 2.5 — Liquid Glass & Adaptive Interface

9 Kasım 2026, Europe/Istanbul. `development`, HEAD `f155c5c`; başlangıçta önceki oturumun commit edilmemiş Phase 2 değişiklikleri vardı ve korundu. Commit/push/release/installer/publish yapılmadı. Bu rapor Phase 2 raporunun devamıdır.

## Tasarım ve uygulanmış davranış

İki ayrı gerçek WPF ControlTemplate hazırlandı: **Tidal**, tek ve kesintisiz yüzen kapsül; **Prism**, kapak ve kontrolleri ayrı adacıklara bölen düzen. Kaynak: `docs/design/phase25/Variants.xaml`. İkisinin açık/koyu renderları teslim görsellerindedir. Daha az sınır ve daha küçük görev çubuğu izi için Tidal seçildi; Prism üretim arayüzüne eklenmedi. Tasarım çalışmaları uygulama BAML paketine alınmıyor.

**Compact:** 32 DIP kapak, mevcut marquee başlığı/sanatçı, kullanıcının tercih ettiği play/pause ve yer varsa next. Üç nokta düğmesi açıkça kontrol panelini açar. Daha dar alanda önce üç nokta, sonra next gizlenir; **kapak her zaman kontrol panelini açan bir düğmedir**. Temel sürüm sığmazsa eski konumlandırma akışı widget'ı gizler. Play kapalı tutulabilir. Normal tüm kontroller açıkken kapsül 298×41 DIP; önceki satır 413×43 DIP idi. Minimum genişlik play açıkken 152, kapalıyken 118 DIP. Dış konumlandırma/anchor matematiği yeniden yazılmadı.

**Expanded:** yalnız tıklamayla açılır; hover panel açmaz. Ayrı WPF Popup içinde büyük kapak, bağlı başlık/sanatçı, previous/next, play/pause, shuffle/repeat, favori, ses ve timeline vardır. Kontroller eski medya handler'larını kullanır. Kapak `VisualBrush` ile mevcut kapak/fallback/crossfade ağacını gösterir; ikinci bir medya servisi, kapak decode veya polling akışı yoktur. İkinci play/next düğmeleri aynı handler ve mevcut ikon durumuna bağlıdır. Bütün Show* tercihleri panelde de uygulanır; gizli bir kontrol flyout'ta yeniden gösterilmez. Başlığa tıklamakla Spotify'ı açma davranışı korunur; paneldeki Spotify düğmesi aynı mevcut açma eylemini kullanır.

Panel dış tıklamayla veya kapatma düğmesiyle kapanır; klavye olayı panele ulaştığında Escape de kapatır. Ana pencere/flyout odak almaz. Panel açıkken mevcut auto-hide pin koşulu uygulanır ve ana pencerenin topmost yeniden vurgusu durur. Widget gizlendiğinde, monitor owner'ı değiştiğinde, ayar değişiminde, taşıma moduna girildiğinde ve kapanışta panel kapatılır. İçteki ses popup'ı dış paneli açık tutar; pointer iki yüzeyi de terk ettiğinde mevcut ses watchdog/leave akışı bunları kapatır. Kapanan panelin bekleyen volume okuması sonradan popup açamaz. Kullanıcı dış tıklamayla başka uygulamaya geçtiyse expanded kapanışı eski odağı zorla geri getirmez.

## Materyal: gerçek blur ayrımı

Üretimde kullanılan efekt **WPF ile çizilen yarı saydam cam benzetimidir**: temaya özel alpha gradient yüzey, üst/sol tarafta düşük opaklıklı radial yansıma, tek DIP değişken ışık kenarı ve iç yüzey tonu. Gerçek arka plan pikselleri örneklenmez veya bulanıklaştırılmaz. `BlurEffect`, ekran yakalama, sürekli efekt render döngüsü, shader paketi ve native Acrylic/Mica çağrısı eklenmedi. Arka plan yalnız normal pencere alpha compositing yoluyla görünür. Kapsül ve panelde dış glow/ağır gölge yok; önceki context menu/ses popup stilleri korunur.

Koyu tema soğuk dumanlı slate tonları ve açık kenar yansıması kullanır. Açık tema ayrı beyaz/buz mavisi katmanlar ve koyu mürekkep kullanır. Bu paletler `Themes/Dark.xaml` / `Themes/Light.xaml` içindedir. Yeni 120–180 ms opacity ve 5 DIP panel giriş hareketi layout ölçüsünü animasyonla değiştirmez. Windows `ClientAreaAnimation=false` olduğunda yeni animasyonlar kaldırılır. Eski kapak crossfade/marquee/auto-hide yolları değiştirilmedi.

### Native seçenek değerlendirmesi

| Seçenek | Kanıt / uyumluluk değerlendirmesi | Karar |
|---|---|---|
| Mevcut WPF pencere | `MainWindow.xaml`: AllowsTransparency, WindowStyle=None, Topmost ve ShowActivated=false. Win32 owner/taskbar, px↔DIP, region kırpma ve hook'lar mevcut. WPF kaynak kodu AllowsTransparency'yi UsesPerPixelOpacity'ye aktarır | Bu yol korundu; HWND/render mimarisi değiştirilmedi |
| DWM backdrop | Belgelenmiş MAINWINDOW Windows 11'de Mica, TRANSIENTWINDOW Desktop Acrylic seçer. Bu API'nin varlığı mevcut per-pixel-alpha taskbar penceresinde sonucu kanıtlamaz | Bu pencere/owner/karma DPI kombinasyonunda native backdrop doğrulanmadı; destekleniyor veya olanaksız diye raporlanmıyor |
| Windows App SDK Acrylic | Controller/composition target ve ilave SDK/dağıtım yaşam döngüsü gerektirir. Bu projede bu bağımlılık yok | Sadece efekt için bağımlılık ve pencere entegrasyonu eklenmedi |
| Windows.UI.Composition | Microsoft'un WPF örneği HwndHost/child HWND, dispatcher queue ve COM composition target yönetir | Ayrı görsel/hit-testing/lifetime katmanı eklemek bu kapsam için gereksiz risk; mevcut katmanlı WPF fallback seçildi |

Kaynaklar: [WPF Window kaynak kodu](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Window.cs), [DWM backdrop enum](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwm_systembackdrop_type), [Mica/Acrylic belgeleri](https://learn.microsoft.com/en-us/windows/apps/develop/ui/system-backdrops), [WPF Composition entegrasyonu](https://learn.microsoft.com/en-us/windows/uwp/composition/using-the-visual-layer-with-wpf). Materyal seçimi mevcut projeye ilişkin mühendislik kararıdır; sentetik PNG native blur kanıtı sayılmaz.

## Derleme ve regresyon

| Kontrol | Başlangıç | Son |
|---|---|---|
| Debug | 0 uyarı / 0 hata | 0 uyarı / 0 hata |
| Release | 0 uyarı / 0 hata | 0 uyarı / 0 hata |
| xUnit | 13/13 | 17/17, 0 atlandı |

Mevcut dokuz servis/veri/ayar testi korundu. UI testleri uyarlanan kompakt davranışı ve genişletilmiş paneli doğrular; dört ek test tasarım varyasyonlarını, tüm görünürlük tercihlerinin kapanmasını/giriş animasyonu temizliğini, timeline bağını ve native popup host'unu kapsar. Gerçek medya komutları test diye tetiklenmedi.

- Açık/koyu palet ve alt menü kaynakları; state/Unknown/iyimser renklerin korunması.
- 100/125/150/200% render çözünürlüğünde iki tema, kapak/fallback, dar kapsül ve expanded: 32 widget/panel PNG, 4 mevcut flyout, 4 tasarım çalışması.
- 0.8/1.0/1.1 kullanıcı ölçeğinde gerçek Root LayoutTransform ile sığma/bant yüksekliği; dar alanda kapaktan erişimin korunması.
- ShowPlay/Next/Prev/Like/Shuffle/Repeat/Volume ve progress; %20 opaklığın panelde korunması; timeline aynı mevcut position/duration ve seek track'ini kullanır.
- Kontrol adları mevcut tooltip/state metnini izleyen UI Automation binding'i kullanır; Invoke pattern'ı doğrulanır. Test bu pattern'ı çağırıp medya durumunu değiştirmez.
- Yeni VSM süreleri ve reduced-motion; panel kapanışında giriş clock'larının kaldırılması.

**Native testin sınırı:** sentetik ayarlarla kurulan MainWindow **gösterilmedi**. Gerçek görsel ağacı ayrı, NOACTIVATE WPF host'una taşındı; MainWindow Loaded, medya, taskbar owner/hook/timer ve startup işlemleri çalıştırılmadı (`_uiReady=false` doğrulandı). Host yalnız kendi HWND'sini açtı. Gerçek expanded ve ses Popup HWND'leri açılıp kapatıldı; foreground değişmedi, WS_EX_NOACTIVATE vardı, nested capture durumu geri döndü ve HideWidget ikisini kapattı. Bu test gerçek Windows popup testidir; **canlı Spotify/görev çubuğu/karma monitör testi değildir**. Ekran dışı renderlar masaüstü ekran görüntüsü değildir.

Son otomatik kayıt `obj/phase25/test-results/phase25.trx`. Önceki oturumda belirlendiği gibi VSTest süreç haberleşmesi için izinli test çalıştırması kullanıldı. Testler gerçek kullanıcı ayarını okumaz/yazmaz; Registry veya Windows taskbar ayarı değiştirilmez.

```powershell
$env:DOTNET_CLI_HOME = Join-Path $PWD 'obj\cli-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
dotnet build SpotifyTaskbarWidget.csproj -c Debug --no-restore
dotnet build SpotifyTaskbarWidget.csproj -c Release --no-restore
dotnet test tests/SpotifyTaskbarWidget.Tests/SpotifyTaskbarWidget.Tests.csproj --no-restore
```

## Render maliyeti

Başlangıçtaki **yerel Phase 2 Release DLL'i** bu oturumun başında `obj/phase25/baseline/bin` altına alındı; Git HEAD'inin eski UI'siyle karıştırılmadı. Aynı bağımlılıksız probe, her sürümü ayrı süreçte yükledi. Constructor'dan önce Shared alanına sentetik ayar verildi; pencere gösterilmedi, Loaded/medya/taskbar hook/timer'ları çalışmadı. Koşul: tek sanal widget/panel, koyu tema, 96 DPI; önce 60 çizim ve 2 saniye ısınma, sonra üç adet yaklaşık 5 saniyelik örnek. Her karede yeniden kullanılan RenderTargetBitmap temizlenip çizildi, 16 ms beklendi. Windows zamanlaması yaklaşık 31.6–31.8 kare/sn üretti. Son ölçüm sırasında build/test çalıştırılmadı; ilk keşif ölçümü yerine sessiz tekrarın sonuçları aşağıdadır.

| Ölçüm | Önce / Phase 2 satırı | Phase 2.5 compact | Phase 2.5 expanded |
|---|---|---|---|
| Render alanı | 413×43 DIP | 298×41 DIP | 316×225 DIP |
| Süre / kare | 15.012 sn / 474 | 15.075 sn / 476 | 15.049 sn / 479 |
| Ortalama CPU, tek çekirdek | %2.19 | %2.28 | %2.60 |
| Ortalama working set | 99.13 MiB | 99.23 MiB | 114.20 MiB |
| Ortalama private memory | 51.40 MiB | 52.04 MiB | 62.59 MiB |

Makine 20 mantıksal işlemci bildiriyor. CPU tek çekirdeğe göre normalize edildi; tüm makine yüzdesi için 20'ye bölünmeli. Compact CPU farkı yaklaşık +0.09 yüzde puanı, working set +0.10 MiB. Expanded daha büyük bir yüzeydir ve ilk ölçüme göre working set yaklaşık +15.07 MiB'dir. Aynı sayıda piksel çizilmediği için bu bir eşit alanlı shader benchmark'ı değildir; gerçek iki kullanıcı durumunun çizim maliyetini örnekler. Kompakt görünümün process'i panel nesnelerini içerir; expanded örneği panelin ölçülmüş/render edilmiş hâlidir.

Kısa örneklerde CPU oynaklığı yüksektir (örnek aralıkları: önce %1.25–3.75, compact %0.93–4.05, expanded %0.62–5.31 / tek çekirdek). Bu sonuç canlı idle/oynatma, GPU/DWM maliyeti, sızıntı veya uzun süreli performans garantisi değildir. Gerçek kontroller, kapak geçişleri ve medya yükü ayrıca ölçülmelidir. Probe kodu ve ham veriler: `obj/phase25/probe/`, `perf-before.json`, `perf-compact.json`, `perf-expanded.json`. Bunlar Git tarafından yok sayılır.

## Görseller ve teslim dosyaları

`docs/screenshots/phase25/` içinde iki tasarımın iki teması, seçilen kapsül/panelin iki teması, dar alan/fallback örnekleri ve genel karşılaştırma vardır. Yeniden üretilen tüm ölçekler `obj/phase25/visuals/` altındadır. Önceki Phase 2 görselleri korunmuştur.

- [İki tasarım çalışması](screenshots/phase25/studies.png), [seçilen compact + expanded önizlemesi](screenshots/phase25/preview.png).
- Önce: [koyu](screenshots/phase25/before_dark.png), [açık](screenshots/phase25/before_light.png).
- Compact: [koyu](screenshots/phase25/dark_compact.png), [açık](screenshots/phase25/light_compact.png).
- Expanded: [koyu](screenshots/phase25/dark_expanded.png), [açık](screenshots/phase25/light_expanded.png).
- Dar alan: [koyu](screenshots/phase25/dark_narrow.png), [açık](screenshots/phase25/light_narrow.png).
- Kapaksız: [koyu](screenshots/phase25/dark_fallback.png), [açık](screenshots/phase25/light_fallback.png).

Görseller WPF RenderTargetBitmap çıktısıdır; genel önizleme bu gerçek renderları yan yana birleştirir. İki tema ve farklı çözünürlükler gözle incelendi. Fiziksel ekran görüntüsü veya native blur doğrulaması olarak sunulmaz.

Ürün: `MainWindow.xaml`, `MainWindow.xaml.cs`, yeni `MainWindow.Adaptive.cs`, `Themes/Dark.xaml`, `Themes/Light.xaml`, üç EN/PT metni için `L.cs`; tasarım XAML'ini paketten dışlamak için `SpotifyTaskbarWidget.csproj`. Test: `UiVisualSnapshotTests.cs`; Phase 2'nin sentetik constructor ve `Properties/AssemblyInfo.cs` desteği korundu. Belge: bu rapor, devir belgesi, iki XAML çalışma şablonu ve PNG'ler. Build/probe çıktıları `obj/` altında, Git tarafından yok sayılır. Yeni paket/framework yok.

## Kalan kabul kontrolleri

- **Doğrulanan:** XAML/tema renderı, daha küçük görev çubuğu izi, ayar görünürlükleri, reduced-motion, native izole host'ta odak ve popup kapanışı.
- **Kodda korunan / canlı denenmeyen:** mevcut medya handler'ları, UIA/CoreAudio fallback, crossfade, auto-hide/fullscreen, Explorer/monitor yeniden oluşturma, manuel konum/migrasyon. Expanded pin/cleanup bu mevcut akışlara eklendi; gerçek taskbar davranışı test sonucu olarak sunulmuyor.
- **Test edilmemiş:** fiziksel karma DPI/negatif monitor koordinatları, gerçek Spotify etkileşimleri, uzun süreli CPU/private bytes/handle eğilimi ve native blur. Spotify süreç kontrolünde kapalıydı ve başlatılmadı. Sonraki kabul adımı Spotify'ın açık olduğu ayrı profil/VM'de taskbar regresyon matrisi ve 30 dakikalık kullanım ölçümüdür.
