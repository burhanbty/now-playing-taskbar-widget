# Phase 2 — Fluent UI uygulama ve doğrulama

9 Kasım 2026, Europe/Istanbul. Başlangıç: temiz `development`, HEAD `f155c5c4c1d047208613741247b47e7a4a319723`. Bu commit zaten ilk Fluent tasarımını içeriyordu; bu çalışma kalan tema ve etkileşim eksiklerini tamamlar. Commit, push, publish, installer veya release çalıştırılmadı.

## Değişiklikler

- `Themes/Dark.xaml` ve `Themes/Light.xaml`: metin, yüzey, kenarlık, hover, slider ve play renkleri tek paletten çözülür. Açık temadaki menü/alt menü ve slider artık sabit koyu renk kullanmaz. Popup kaynakları ana pencereyle paylaşılır.
- `MainWindow.xaml`: sabit 26/30 DIP buton alanları içinde 140 ms opacity geçişleri, hover ve basılma durumları; play önceliği korunur. Play gölgesi ve ani ikon büyütmesi kaldırıldı. Segoe UI Variable başlık 12.5 DIP, sanatçı 11 DIP; ikincil metin kontrastı artırıldı. Albüm kapağı için kaliteli ölçekleme ve fonta bağlı olmayan vektör fallback eklendi; iki kapak katmanı, yuvarlak ImageBrush ve UniformToFill korundu.
- `MainWindow.xaml.cs`: aynı Registry sistem teması kontrolü paletleri değiştirir. Yerel durum fırçaları yeniden renklendirilirken aktif/Unknown/iyimser durum ve tooltip metni korunur. Yeni buton geçişleri ve mevcut flyout fade'i Windows `ClientAreaAnimation` tercihinde kapatılır; yeni timer veya event aboneliği eklenmez. Mevcut kapak crossfade, marquee ve auto-hide animasyonları değiştirilmedi.
- UI testi için sentetik ayar kabul eden internal constructor ve `Properties/AssemblyInfo.cs` eklendi. Genel constructor aynı Shared ayar yolunu kullanır. Testler pencereyi **göstermez**, Loaded akışını başlatmaz ve `ClosedByApp` ile kapanışta yeniden oluşturmayı engeller. Gerçek ayar dosyası fixture olarak kullanılmaz.

Medya servisleri, Win32/COM entegrasyonu, konumlandırma algoritması, ayar migrasyonu, kontrol handler'ları ve görünürlük önceliği değiştirilmedi. Yeni bağımlılık yok.

Değişen/eklenen dosyalar: `MainWindow.xaml`, `MainWindow.xaml.cs`, `Themes/Dark.xaml`, `Themes/Light.xaml`, `Properties/AssemblyInfo.cs`, `tests/SpotifyTaskbarWidget.Tests/UiVisualSnapshotTests.cs`, `docs/AI_HANDOFF.md`, bu rapor ve `docs/screenshots/phase2/` içindeki 11 PNG. `obj/phase2/` altındaki test, render ve probe çıktıları Git tarafından yok sayılır.

## Derleme ve otomatik doğrulama

| Kontrol | Başlangıç | Sonuç |
|---|---|---|
| Uygulama Debug | 0 uyarı, 0 hata | 0 uyarı, 0 hata |
| Uygulama Release | 0 uyarı, 0 hata | 0 uyarı, 0 hata |
| xUnit | 10/10 geçti | 13/13 geçti, 0 atlandı |

Eski snapshot testi iyileştirildi; üç yeni UI regresyon testi eklendi. Mevcut dokuz yaşam döngüsü/veri/ayar testi korundu. UI doğrulaması gerçek XAML ve gerçek tema uygulama metodunu kullanır:

- Koyu → açık → koyu palet değişimi, menü ve alt menünün implicit stilleri/yüzeyleri, aktif shuffle ve Unknown repeat renginin korunması.
- 0.8/1.0/1.1 kullanıcı ölçeğinde minimum sığma sınırı, kontrol gizleme önceliği, seçili kontrollerin gizli kalması ve 48 DIP bandını aşmayan sentetik ölçüm.
- Yeni geçişlerin 140/0 ms olması, VSM basılma storyboard'unun deterministik değerlendirmesi, popup animasyon tercihi ve odak çalmama XAML bayrakları. Bu, gerçek mouse/klavye veya z-order testi değildir.
- %100/125/150/200 bitmap çözünürlüğünde iki tema, kapak/fallback ve dar görünüm: 24 widget + 4 flyout PNG. Bu, fiziksel monitör DPI geçişi testi değildir.

İlk sandbox VSTest çalıştırmaları süreç bağlantısında bekledi; datacollector bağlantısı 90 saniyede zaman aşımına uğradı. İzinli çalıştırmada başlangıç ve son testler geçti. Görsel test sırasında yakalanan `VisualTransition.GeneratedDuration` DynamicResource hatası ve popup kaynak çözümleme eksikliği giderildi. Son sonuç: `obj/phase2/test-results/phase2.trx`.

Tekrar çalıştırma, proje kökünde:

```powershell
$env:DOTNET_CLI_HOME = Join-Path $PWD 'obj\cli-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
dotnet build SpotifyTaskbarWidget.csproj -c Debug --no-restore
dotnet build SpotifyTaskbarWidget.csproj -c Release --no-restore
dotnet test tests/SpotifyTaskbarWidget.Tests/SpotifyTaskbarWidget.Tests.csproj --no-restore
```

Önbellek yoksa önce uygulama ve test projelerini restore edin. Snapshot çıktıları test sırasında `obj/phase2/visuals` altına üretilir; takipli görseller test tarafından değiştirilmez.

## Görsel kanıt

**Canlı görev çubuğu ekran görüntüsü alınmadı.** Spotify süreç kontrolünde çalışmıyordu. Spotify, widget'ın gerçek startup akışı veya medya başlatılmadı; mevcut profil/Registry/taskbar ayarlarına müdahale edilmedi.

Başlangıç görselleri eski testin sentetik, şeffaf arka planlı çıktılarıdır; sonraki görseller gerçek yeni XAML'in sentetik taskbar arka planına çizimidir. Parça metni ve kapak fixture'ları farklıdır; bunlar piksel farkı testi olarak kullanılmamalı.

- Önce: [koyu](screenshots/phase2/before_dark.png), [açık](screenshots/phase2/before_light.png).
- Sonra: [koyu](screenshots/phase2/dark.png), [açık](screenshots/phase2/light.png), [önizleme](screenshots/phase2/preview.png).
- Dar alan: [koyu](screenshots/phase2/dark_compact.png), [açık](screenshots/phase2/light_compact.png).
- Menü: [koyu](screenshots/phase2/dark_menu.png), [açık](screenshots/phase2/light_menu.png).
- Ses popup'ı: [koyu](screenshots/phase2/dark_volume.png), [açık](screenshots/phase2/light_volume.png).

Önizlemeler ve farklı ölçek çıktıları gözle incelendi; açık tema metinleri/ikonları koyu, koyu tema metinleri/ikonları açık. Yuvarlatılmış kapak, fallback, kompakt alan, menü ve ses popup'ında görünür taşma gözlenmedi. Uzun başlık dar alanda kliplenir; gerçek marquee davranışı değiştirilmedi ve canlı denenmedi.

## Sınırlı çizim maliyeti karşılaştırması

Başlangıç HEAD'i `git archive` ile `obj/phase2/baseline-source` içine çıkarılıp ayrı derlendi. Bu geçici baseline restore'unda ağdan NuGet güvenlik verisi alınamadığı için **NU1900** uyarısı vardı; derleme başarılıydı. Yukarıdaki uygulama Debug/Release sonuçları bu geçici build'den ayrıdır.

`obj/phase2/probe` altındaki bağımlılıksız WPF probe iki Release DLL'ini ayrı süreçlerde yükledi. Her iki sürümde de Shared alanına **constructor'dan önce sentetik ayar** verildi. Pencere gösterilmedi, Loaded/medya/timer/hook başlatılmadı. Koşul: tek sanal widget, koyu tema, 96 DPI, 413×43 DIP, kapaksız sentetik parça, 60 çizim ısınması, aynı RenderTargetBitmap üzerinde 16 ms beklemeli çizim döngüsü, sürüm başına üç adet yaklaşık 5 saniyelik örnek. Windows zamanlamasıyla her örnekte 159 kare, yaklaşık 31.7 kare/sn oluştu. Makine 20 mantıksal işlemci bildiriyor. CPU **tek çekirdeğe göre** normalize edildi; tüm makine yüzdesi için 20'ye bölünmeli.

| Ölçüm | Başlangıç | Phase 2 |
|---|---|---|
| Toplam süre / kare | 15.041 sn / 477 | 15.064 sn / 477 |
| Toplam CPU / süre | %5.09 tek çekirdek | %6.02 tek çekirdek |
| Ortalama working set | 111.37 MiB | 112.47 MiB |
| Ortalama private memory | 60.28 MiB | 60.75 MiB |

Ham veriler: `obj/phase2/perf-before.json`, `obj/phase2/perf-after.json`. Süreç kapanışları probe'un kendi `finally` bloğunda gerçekleşti; mevcut uygulamalara müdahale edilmedi. Working set farkı yaklaşık +1.10 MiB, CPU farkı +0.93 yüzde puanı/tek çekirdektir. Kısa örneklerde CPU oynaklığı yüksektir; bu sonuç canlı performans regresyonunu dışlamaz, sızıntı veya performans garantisi sağlamaz. Başlangıçtaki gizli gerçek uygulama ölçümleriyle karşılaştırılmamalıdır.

## Kalan doğrulama ve sonraki adım

**Giderilen doğrulanmış UI eksikleri:** açık temadaki sabit koyu flyout/slider renkleri; anlık hover/pressed ölçek sıçramaları; fonta bağlı kapak fallback'i. Yeni tasarım sentetik WPF ortamında doğrulandı.

**Kodda korunan fakat canlı test edilmemiş davranışlar:** medya komutları, kapak crossfade, marquee, odak/z-order, auto-hide/fullscreen, fiziksel monitör geçişleri, karma DPI/negatif koordinatlar, monitör başına ManualX ve ayar kalıcılığı. Bunlar otomatik test başarısından çıkarılmamalı.

**Test edilmemiş varsayım:** gerçek Spotify yükü altında uzun süreli CPU/RAM eğiliminin kabul edilebilir kalması. Sonraki adım, Spotify'ın açık olduğu ayrı Windows profili/VM'de mevcut temel regresyon matrisini ve 30 dakikalık idle/oynatma ölçümünü tamamlamak. Yeni özellik fazına geçmeden bu canlı kabul kontrolü önerilir.
