# Phase 2.6 — Gerçek Windows doğrulama kaydı

İlk doğrulama **9 Kasım 2026**, SimpMusic takibi **10–12 Kasım 2026** (Europe/Istanbul). Çalışma dizini proje köküdür.

## Güncel sonuç — SimpMusic

Kullanıcı önce Spotify yerine SimpMusic ile test istedi, ardından mevcut kapsamı genişleterek SimpMusic desteğinin eklenmesini onayladı. **SimpMusic ile gerçek Release widget'ı Windows 11 görev çubuğunda görünür oldu.** Gerçek medya oturumunda oynat/duraklat, sonraki/önceki ve uygulama sesi çalıştı. Debug ve Release derlendi, **22/22 xUnit testi geçti**. Pencere tıklamalarına bağlı kalan kontroller aşağıda belirtildi. Bu bölüm 9 Kasım'daki ilk turdan sonraki durumu anlatır.

### Neden ve değişiklik

10 Kasım'daki tanıda SimpMusic 2.5.0.0, Windows'a `Simpmusic_ejp2bhxmz1qq6!Simpmusic` kimliğiyle `Playing` oturumu yayımladı. Eski sürüm bu oturumu seçmedi. `MediaService` Spotify kimliği arıyor, `MainWindow` ayrıca `Spotify.exe` varlığını istiyordu. Gerçek eski Release süreci çalışıyor ve görünür hedef sunmuyordu. Tanı kaydı `obj/phase26/live-media-sources.json`.

- `MediaService.cs` doğrulanmış Store SimpMusic kimliğini seçime ekler. Spotify oturumu varsa önceliğini korur. Diğer medya kaynakları seçime girmez. Okuma ve komutlar seçili oturum referansına bağlıdır. Oturum değişirse eski sonuçlar uygulanmaz.
- `MainWindow.xaml.cs`, `MainWindow.Adaptive.cs`, `MainWindow.xaml` ve `L.cs` etkin oynatıcıya göre kontrol görünürlüğünü, panel etiketini ve tıklamayla açılan oynatıcıyı belirler. SimpMusic için taşıma düğmeleri SMTC destek bayraklarına, shuffle ve repeat ayrıca okunabilir durum bilgisine bağlıdır. Geçerli süre yoksa timeline gizlidir. Favori kontrolü Spotify yolunda kalır. SimpMusic metadata akışı Spotify UI Automation taramasına girmez. [Microsoft SMTC kontrol bayrakları](https://learn.microsoft.com/en-us/uwp/api/windows.media.control.globalsystemmediatransportcontrolssessionplaybackcontrols)
- `SpotifyVolume.cs` aynı CoreAudio yolunu etkin oynatıcının süreç kimlikleriyle sınırlar. SimpMusic ses komutları `SimpMusic.exe` ses oturumuna gider. `SpotifyActions.cs` açma eyleminde çalışan doğru oynatıcı penceresini bulur. Liquid Glass yüzeyleri, görev çubuğu konumlandırma algoritması ve ayar şeması mevcut hâliyle kullanılır.
- `MediaDataTests.cs` ve `UiVisualSnapshotTests.cs` SimpMusic kimliği, Spotify önceliği, yabancı kaynak reddi ve oynatıcıya göre düğme görünürlüğü için beş test ekler.

### 12 Kasım canlı kabul matrisi

Windows 11 `10.0.26300`, tek ekran, koyu tema, widget için gerçek **96 DPI**. Sadece yerel geliştirme Release EXE'si çalıştırıldı. `EnumWindows` ve pencere bilgileri görünür HWND'yi doğruladı. `Shell_TrayWnd` sınırı `(0,1032)–(1920,1080)`, widget sınırı `(12,1035)–(310,1076)`. Widget **298×41 fiziksel piksel**, owner `Shell_TrayWnd` ve `WS_EX_NOACTIVATE` etkin. Son Release yeniden başlatma sonrasında aynı sınırlar gözlendi.

| Senaryo | Sonuç |
|---|---|
| Gerçek görev çubuğu yerleşimi / compact | **Geçti, pencere bilgisiyle.** Widget görev çubuğunun 48 px bandının içinde ve görünür. Görsel çakışma, metin okunabilirliği ve hover değerlendirmesi açık. |
| Expanded aç/kapat / popup odağı | **Canlı ürün tıklaması doğrulanamadı.** Computer Use, görünür ve görev çubuğuna ait widget HWND'sini hedef envanterine almadı. Otomatik izole HWND testi geçti. Ana pencerenin gerçek `WS_EX_NOACTIVATE` bayrağı okundu. |
| Play/Pause | **Geçti, gerçek SMTC oturumunda.** Ürün DLL'indeki aynı komut metoduyla `Playing → Paused → Playing`, ardından test sonu `Paused` okundu. |
| Next/Previous, metadata güncellemesi | **Geçti, gerçek SMTC oturumunda.** Next başlık/sanatçı anahtarını değiştirdi, Previous test başlangıcındaki parçayı geri getirdi. |
| Albüm kapağı | **Kaynak verisi doğrulandı.** Gerçek oturum başlık, sanatçı ve thumbnail yayımladı. Canlı crossfade gözlemlenmedi. |
| Progress / seek | **Sağlayıcı verisi yetersiz.** SimpMusic oynarken ve duraklatılmışken `Position=0`, `Duration=0` bildirdi. Seek bayrağı mevcut olsa da ölçülebilir timeline oluşmadı. |
| Ses | **Geçti, gerçek CoreAudio oturumunda.** Ürün DLL'indeki aynı ses yoluyla %100 → %95 → %100 okundu. Test sonu düzey geri yüklendi. Slider tıklaması ayrıca denenemedi. |
| Shuffle / Repeat / Favorite | **Bu SimpMusic oturumunda doğrulanamadı.** Shuffle/Repeat bayrakları açık, durumları boştu. Son derleme bu düğmeleri durum bilgisi geldiğinde gösterir. Favori Spotify UIA yolunda kalır. |
| Fullscreen, auto-hide, açık tema, karma DPI | **Test edilemedi.** Aktif kullanıcının görev çubuğu, tema ve ekran ayarları değiştirilmedi. |
| Kapat / yeniden aç | **Süreç yeniden başlatması geçti.** Yolu doğrulanan test PID 29992 kapatıldı ve son Release PID 27636 ile görünür görev çubuğu penceresi tekrar oluştu. Menüden normal çıkış ayrıca denenemedi. |

Taşıma ve ses testleri ürün DLL'indeki yöntemleri gerçek medya oturumuna karşı çalıştıran yerel tanı üzerinden yapıldı. Bu yöntem widget düğmelerinin fiziksel tıklaması olarak sayılmadı. Test başlangıcındaki parça ve ses düzeyi geri yüklendi, SimpMusic duraklatıldı. Son Release widget oturumda açık bırakıldı. Kullanıcı ayar dosyası oluşmadı.

**Ekran görüntüsü.** SimpMusic'in gerçek penceresi gözlendi. Widget, Computer Use hedef listesinde yer almadığı için gerçek widget ekran görüntüsü alınamadı. Kütüphane ekranı ve kişisel parça adları depoya kaydedilmedi. Widget'ın görünürlüğü, konumu ve boyutu Win32 kayıtlarıyla belgelenmiştir.

### Canlı performans ve derleme

| Durum | Süre | CPU, tek çekirdek | Working set | Private memory | Handle |
|---|---:|---:|---:|---:|---:|
| Eski Release, SimpMusic açık, widget gizli | 15,06 sn | %0,21 | 220,25 MiB | 130,33 MiB | 685 |
| SimpMusic duraklatılmış, compact görünür | 8,02 sn | %0,39 | 312,43 MiB | 205,36 MiB | 773 |
| SimpMusic çalıyor, compact görünür | 8,00 sn | %1,17 | 313,17 MiB | 205,87 MiB | 776 |
| Expanded | — | — | — | — | — |

Compact örnekleri aynı gerçek widget sürecinde, bir ekranda ve 20 mantıksal işlemcili ortamda ölçüldü. CPU süreç zaman farkının örnek süresine bölümüdür. Bellek ve handle sütunları örnek sonu değerleridir. Eski gizli süreç farklı görünürlük koşulunda çalıştığı için doğrudan bellek regresyonu karşılaştırması yapılamaz. Kısa örnekler uzun süreli eğilim veya sızıntı kanıtı değildir.

- Son Debug ve Release başarılı, **0 hata ve her birinde 1 `NU1900`**. NuGet güvenlik açığı verisine erişemedi. İlk tanı derlemesi çalıştırdığım EXE'yi yeniden derlemeye çalışırken `MSB3027/MSB3021` dosya kilidine takıldı. Tanıyı mevcut DLL'e bağlayınca çalıştı. Son doğrulama, önce test EXE'si kapatılarak yapıldı.
- Son xUnit **22/22 geçti**, 0 atlandı. Kayıt `obj/phase26/test-results/simpmusic.trx`. Loglar `simpmusic-debug.log`, `simpmusic-release.log`.
- Gerçek kanıt dosyaları `obj/phase26/simpmusic-live-windows.json`, `simpmusic-transport.json`, `simpmusic-final-live.json`, `simpmusic-restart-process.json` ve `live-hidden-widget-performance.json`. Probe ve ham kayıtlar `obj/` altında Git dışında tutulur. Ürün değişiklikleri yerel çalışma ağacındadır.

## İlk tur sonucu — 9 Kasım

**Tam canlı görev çubuğu kabulü tamamlanamadı.** Gerçek Windows oturumuna erişildi; ancak Spotify ve widget çalışmıyordu. Kullanıcının “Spotify kapalıysa kullanıcı onayı olmadan uygulamayı başlatma veya müzik çalma” koşulu nedeniyle iki uygulama da başlatılmadı. Başlatma onayı soruldu; bu rapor hazırlanırken onay alınmadı. Aşağıdaki otomatik test başarısı, çalışan Spotify ile ürünün görev çubuğunda doğrulandığı anlamına gelmez.

Yeni özellik, Liquid Glass tasarım değişikliği, ürün kodu düzeltmesi, commit, push, publish, installer veya release yapılmadı. Önceki Phase 1/2/2.5 yerel değişiklikleri korundu.

## Hazırlık ve ortam kanıtı

- `AGENTS.md`, `docs/AI_HANDOFF.md`, `docs/PHASE2_VALIDATION.md` ve `docs/PHASE25_VALIDATION.md` okundu.
- Git: `development...origin/development`, HEAD `f155c5c4c1d047208613741247b47e7a4a319723`. Başlangıçta Phase 2/2.5'e ait takipli değişiklikler ve yeni dosyalar vardı; reset/checkout/clean yapılmadı. `origin` kullanıcının fork'u, `upstream` orijinal depodur; uzak depolara yazılmadı.
- Gerçek host sürümü: **Windows NT 10.0.26300.0**. 20 mantıksal işlemci bildirildi. Bu sürüm bilgisi mevcut oturumdan alındı; yalnız eski rapordan kopyalanmadı.
- Hem normal hem izinli salt okunur süreç kontrolü: **Spotify 0, SpotifyTaskbarWidget 0 süreç**. Computer Use `list_apps` / `list_windows` envanterinde de Spotify/widget hedefi bulunmadı. Çalışan widget ile gözlenen çakışma yok; tek örnek mutex'in canlı ikinci başlatma davranışı denenmedi.
- Windows Forms Screen API tek ekran bildirdi: `DISPLAY1`, bounds `{0,0,1920,1080}`, work area `{0,0,1920,1032}`. Bunlar sorgu sürecinin bildirdiği koordinatlardır; fiziksel DPI veya %100 ölçek kanıtı sayılmadı.
- Registry salt okunur tema sorgusu: `SystemUsesLightTheme=0`, `AppsUseLightTheme=0` — koyu tema. Açık tema/ölçek/monitör/taskbar ayarı değiştirilmedi. Auto-hide veya fullscreen davranışı bu bilgilerden çıkarılmadı.
- Gerçek `%APPDATA%\SpotifyTaskbarWidget\settings.json` yoktu. İçeriği okunmadı, fixture yapılmadı, ayar oluşturulmadı veya sıfırlanmadı. Yalnız APPDATA ortam değişkeninin izolasyon sağlamadığı önceki teknik sınıra uyuldu.

Ham ve kişisel içerik taşımayan yerel kayıtlar: `obj/phase26/environment.json`, `display-readonly.json`, `source-hashes.json`. Diğer uygulamaların pencere başlıkları veya hesap bilgileri rapora/depoya alınmadı.

Son kontrolde ürünün 15 kaynak/yapılandırma dosyasının SHA-256 değerleri bu turun başlangıcındaki değerlerle aynıydı; **0 ürün dosyası değişti**. Spotify/widget süreçleri ve ilgili pencere sayısı yine 0, gerçek settings dosyası yine yoktu. Kanıt: `obj/phase26/final-check.json`.

## Derlemeler ve xUnit

| Kontrol | Sonuç | Kanıt |
|---|---|---|
| Debug | Başarılı, 0 uyarı / 0 hata | `obj/phase26/build-debug.log` |
| Release | Başarılı, 0 uyarı / 0 hata | `obj/phase26/build-release.log` |
| xUnit | **17/17 geçti, 0 başarısız, 0 atlandı** | `obj/phase26/test-results/phase26.trx` |
| Whitespace kontrolü | `git diff --check` hata vermedi | Mevcut LF→CRLF bilgilendirmesi build/test hatası değildir |

Komutlar proje kökünde çalıştırıldı; mevcut restore önbelleği kullanıldı:

```powershell
$env:DOTNET_CLI_HOME = Join-Path $PWD 'obj\cli-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
dotnet build SpotifyTaskbarWidget.csproj -c Debug --no-restore
dotnet build SpotifyTaskbarWidget.csproj -c Release --no-restore
dotnet test tests/SpotifyTaskbarWidget.Tests/SpotifyTaskbarWidget.Tests.csproj --no-restore `
  --logger 'trx;LogFileName=phase26.trx' --results-directory obj/phase26/test-results
```

VSTest süreç haberleşmesi için önceki oturumlarda doğrulanan izinli çalıştırma kullanıldı. Testler Windows .NET/WPF üzerinde gerçekten çalıştı; yalnız test sayısı veya önceki sonuç yeniden raporlanmadı.

### Gerçek HWND testi ile canlı ürün testi arasındaki fark

`NativePopup_IsolatedHost_PreservesFocusAndClosesNestedSurface` bu turda yeniden geçti. Sentetik ayarlı MainWindow'un görsel ağacı ayrı bir NOACTIVATE WPF host'unda gösterildi. Gerçek expanded ve volume Popup HWND'leri açıldı; foreground değişmedi, WS_EX_NOACTIVATE doğrulandı, nested popup durumu geri döndü ve HideWidget iki popup'ı kapattı.

Ancak ürünün MainWindow'u gösterilmedi; `_uiReady=false` kontrol edildi. `App.OnStartup`, gerçek taskbar owner/konumlandırma/hook/timer akışı ve Spotify bağlantısı çalıştırılmadı. Bu nedenle bu başarı **izole gerçek Windows popup testi** olarak sınıflandırıldı; Phase 2.6 canlı görev çubuğu/medya kabulü olarak işaretlenmedi.

Diğer UI testleri WPF kaynaklarını ekran dışına render eder ve sentetik state/ayar kullanır. %100–200 PNG'lerin yeniden üretilmesi fiziksel Windows ölçeği veya canlı GUI kanıtı değildir.

## İstenen canlı senaryolar

| Senaryo | Sonuç | Neden / kalan kontrol |
|---|---|---|
| 1. Gerçek görev çubuğuna yerleşme | **Test edilemedi** | Ürün başlatılmadı; Win32 owner, anchor, tray çakışması canlı gözlenmedi |
| 2. Compact görünüm | **Test edilemedi** | Gerçek taskbar üzerinde görünür ürün yoktu |
| 3. Expanded aç/kapat | **Canlı üründe test edilemedi** | Yalnız yukarıdaki izole host testi geçti |
| 4. Play/Pause, Next/Previous | **Test edilemedi** | Spotify/medya oturumu yok; komut gönderilmedi |
| 5. Şarkı ve sanatçı güncellemesi | **Test edilemedi** | Gerçek metadata/track geçişi yok |
| 6. Kapak ve animasyonlar | **Test edilemedi** | Gerçek kapak değişimi/crossfade gözlenmedi |
| 7. Progress ve seek | **Test edilemedi** | Gerçek timeline/seek denenmedi; sentetik state testi ayrı |
| 8. Ses | **Test edilemedi** | Spotify UIA/CoreAudio kontrolü çağrılmadı, ses değiştirilmedi |
| 9. Shuffle, Repeat, Favorite | **Test edilemedi** | Oynatma modu veya müzik kütüphanesi değiştirilmedi |
| 10. Popup'ların odak çalmaması | **Canlı üründe test edilemedi** | İzole host foreground/NOACTIVATE testi geçti; taskbar/Spotify etkileşimi açık |
| 11. Fullscreen / auto-hide | **Test edilemedi** | Ürün yoktu; aktif oturumun taskbar/Explorer ayarları değiştirilmedi |
| 12. Kapat / yeniden aç | **Test edilemedi** | Ürün başlatılmadığı için startup/shutdown/restart döngüsü denenmedi |

Başarısız otomatik test veya yeniden üretilmiş canlı ürün hatası yoktur. **Test edilemedi**, başarılı veya hatasız anlamına gelmez. Uygulamanın görev çubuğunda doğru çalıştığı bu turla kanıtlanmış değildir.

## Görsel inceleme ve ekran görüntüleri

Gerçek görev çubuğunda widget ekran görüntüsü **alınmadı**. Spotify/widget kapalıyken başka uygulamaların ekran görüntülerini kanıt diye toplamak yerine görüntü üretimi yapılmadı. Önceki sentetik görseller bu rapora canlı ekran görüntüsü olarak taşınmadı.

Kapsül/taskbar uyumu, expanded ekran sınırları, gerçek metin okunabilirliği, hover/açılma animasyonları ve açık/koyu gerçek Windows tema geçişleri test edilemedi. Tek monitör dışında bir topoloji gözlenmedi; farklı sistem ölçekleri veya negatif monitör koordinatları denenmedi. Registry, Explorer, taskbar veya ekran ayarları değiştirilmedi.

## Gerçek kullanım performansı

| Durum | CPU | RAM/private bytes | Handle | Sonuç |
|---|---|---|---|---|
| Spotify playing | Ölçülmedi | Ölçülmedi | Ölçülmedi | Gerçek ürün/Spotify çalışmıyordu |
| Spotify paused | Ölçülmedi | Ölçülmedi | Ölçülmedi | Gerçek ürün/Spotify çalışmıyordu |
| Widget expanded | Ölçülmedi | Ölçülmedi | Ölçülmedi | Ürün paneli açılmadı |

Çalışmayan uygulama için sıfır tüketim raporlanmadı. xUnit testhost kaynak kullanımı ürün performansı sayılmadı. Phase 2.5'in kısa ekran dışı render ölçümleri tarihsel/sentetik ölçümlerdir; bu tablonun yerine geçmez. Uzun süreli CPU/private bytes/handle eğilimi veya sızıntı sonucu çıkarılmadı.

## Düzeltmeler ve riskler

**Düzeltilen ürün hatası: yok.** Canlı bir sorun yeniden üretilemediği için tasarım veya kaynak kod değiştirilmedi; gereksiz refactoring/test ilavesi yapılmadı. Bu turdaki yeni dosya bu rapordur; devir belgesine doğrulama özeti eklendi. Derleme/test çıktıları `obj/` altında yok sayılır.

Kalan riskler önceki raporlardaki canlı kabul boşluklarıdır: gerçek taskbar yerleşimi ve popup z-order, ekran sınırları/karma DPI, auto-hide/fullscreen pin temizliği, gerçek medya/state freshness ve UIA/CoreAudio davranışı, kapanış/yeniden açma ve uzun süreli kaynak tüketimi. Bunlar bu turda doğrulanmış yeni arıza olarak sınıflandırılmadı.

Sonraki canlı kabul, Spotify'ın açık/oturum açılmış olduğu ve kullanıcı verisinin korunabildiği ayrı profil/VM veya açıkça ayrılmış ortamda yapılmalı. Playback, ses ve modlar ölçüm öncesi/sonrası kaydedilmeli; Favorite testi gerçek kütüphaneye kalıcı ekleme yapmayan bir test parçasıyla sınırlandırılmalı. Aktif oturumda Explorer restart veya sistem taskbar/ölçek değişikliği yapılmamalı.

## Kapatma isteği

Kullanıcının 9 Kasım'daki kapatma isteği için ilk rapor kaydedildikten sonra normal Windows kapatma komutu gönderildi. `obj/phase26/shutdown-status.json` komutun çıkış kodunu **0** olarak kaydetti. Fiziksel güç durumu bu kayıttan çıkarılmadı. SimpMusic takibi ayrı, daha sonraki Windows oturumunda yapıldı.

Kapatma yardımcısı `obj/phase26/shutdown-after-validation.ps1`, 45 saniye bekleyip `shutdown.exe /s /t 0` çalıştırdı. Kapatma zorlamasız gönderildi. 10 Kasım'daki devamda bekleyen kapatma bulunmadı.
