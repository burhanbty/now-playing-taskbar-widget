# Now Playing Taskbar Widget — Antigravity teknik devir

## SimpMusic desteği ve canlı doğrulama — 12 Kasım 2026

Kullanıcı onayıyla Store SimpMusic (`Simpmusic_ejp2bhxmz1qq6!Simpmusic`) desteği eklendi. Spotify seçimi öncelikli kalır. SimpMusic başlık/sanatçı/kapak ve taşıma için SMTC, uygulama sesi için kendi CoreAudio oturumunu kullanır. Shuffle/Repeat görünürlüğü destek bayrağı ve okunabilir duruma, timeline görünürlüğü geçerli süreye bağlıdır. Favori Spotify'a özgüdür. Oturum değişiminde bekleyen metadata, kapak ve ses güncellemeleri eski oturuma göre uygulanmaz.

Gerçek Windows 11 görev çubuğunda görünür 298×41 widget, taskbar owner ve `WS_EX_NOACTIVATE` doğrulandı. Gerçek SimpMusic oturumunda play/pause, next/previous ve %100 → %95 → %100 ses testi geçti. Son Release süreç yeniden başlatması geçti. Debug/Release 0 hata ve `NU1900`, xUnit **22/22**. Canlı popup tıklamaları, ekran görüntüsü, karma DPI ve fullscreen/auto-hide açık. SimpMusic test edilen oturumda süre/konum ve shuffle/repeat durumu yayımlamadı. Ölçümler, kanıt yolları ve ilk turdan sonraki kapsam değişikliği [Phase 2.6 raporunda](PHASE26_VALIDATION.md).

## Güncel doğrulama — Phase 2.6 (9 Kasım 2026)

[Phase 2.6 canlı doğrulama kaydı](PHASE26_VALIDATION.md) oluşturuldu. Debug/Release 0 uyarı / 0 hata, xUnit **17/17 geçti**. Gerçek Windows 10.0.26300 oturumunda süreç ve masaüstü envanteri Spotify/widget'ın kapalı olduğunu doğruladı. Spotify kapalıyken başlatma için onay alınmadığından ürün/medya başlatılmadı; gerçek görev çubuğu kabulü **tamamlanamadı**. İzole gerçek HWND popup testi yeniden geçti; canlı taskbar/Spotify testi olarak raporlanmadı. Canlı ekran görüntüsü ve playing/paused/expanded performans ölçümü yok. Yeni özellik, görsel değişiklik veya ürün düzeltmesi yapılmadı; önceki değişiklikler korundu. Kullanıcının görev sonu normal bilgisayar kapatma isteği ayrıca raporda kayıtlıdır.

## Güncel ek — Phase 2.5 (9 Kasım 2026)

Phase 2 yerel değişiklikleri korunarak **Tidal cam kapsül + tıklamayla açılan adaptive panel** uygulandı. İki gerçek XAML çalışması (Tidal/Prism) ve açık/koyu renderları üretildi; Tidal seçildi. Materyal WPF alpha/gradyan/ince ışık katmanlarıdır, **native blur/Acrylic/Mica değildir**. Yeni paket/framework yok. Tam teknik değerlendirme, dosyalar, görseller ve doğrulama sınırları: [Phase 2.5 raporu](PHASE25_VALIDATION.md).

- Compact: kapak + mevcut kayan başlık/sanatçı + isteğe bağlı play/next; üç nokta veya kapak paneli açar. Dar alanda üç nokta/next kaldırılır, kapak erişimi korunur. Satır 298×41 DIP (önce 413×43); minimum play ile 152 DIP, playsiz 118 DIP. Dış Win32/DPI/monitor yerleşim algoritması korunur.
- Expanded: mevcut kontroller ve timeline ayrı NOACTIVATE WPF Popup'ındadır; Show* tercihleri korunur. Yeni medya servisleri yok; kapak mevcut görsel/crossfade ağacından gelir. Popup açıkken auto-hide pin ve z-order koruması uygulanır; hide, owner/ayar değişimi, taşıma modu ve kapanış temizliği eklendi. Bekleyen volume okuması panel nesli değişince popup açmaz.
- Son Debug/Release: 0 uyarı / 0 hata. xUnit: **17/17 geçti**. Gerçek popup HWND'si sentetik ayarlı izole host'ta açıldı; foreground/NOACTIVATE, nested volume ve hide temizliği geçti. MainWindow startup/Loaded, gerçek medya ve taskbar hook'ları bu testte çalışmadı.
- 100–200% sentetik render ve 0.8/1/1.1 gerçek LayoutTransform kontrolleri yapıldı; fiziksel karma DPI/taskbar testi değildir. Spotify kapalıydı ve başlatılmadı. Gerçek medya, auto-hide/fullscreen ve uzun süreli kullanım kabul testleri açık.
- Sessiz ekran dışı çizim örneğinde CPU (tek çekirdek) önce %2.19, compact %2.28, expanded %2.60; working set 99.13 / 99.23 / 114.20 MiB. Üçer yaklaşık 5 sn örnek; gerçek oynatma/GPU/sızıntı sonucu değildir. Koşullar ve ham veri yolları rapordadır.

Yeni UI davranışı `MainWindow.Adaptive.cs` içindedir. Yeni ayar şeması/migrasyon eklenmedi. Önceki Phase 2 sonuçları aşağıda tarihsel kayıt olarak korunur. Commit/push/release yapılmadı.

Hazırlık tarihi: **8 Kasım 2026 (Europe/Istanbul)**. **Güncelleme: 9 Kasım 2026, Phase 2.** İlk hazırlıktan sonra Phase 1 ve ilk Fluent tasarımı `f155c5c` commit'ine girdi. Bu oturum mevcut tasarımın açık/koyu paletlerini, popup kaynaklarını, 140 ms buton geri bildirimini, tipografi ve kapak fallback'ini tamamladı. Ayrıntılar, değişen dosyalar, kanıtlar ve sınırlar: [Phase 2 doğrulama raporu](PHASE2_VALIDATION.md). Aşağıdaki hazırlık kayıtları tarihsel başlangıç durumunu anlatır; güncel test durumu öncelikle bu ek rapordan okunmalı.

Güncel sonuç: uygulama Debug ve Release **0 uyarı / 0 hata**, xUnit **13/13 geçti**. 100/125/150/200% sentetik WPF renderları üretildi; canlı Spotify, fiziksel karma DPI, medya kontrolleri ve taskbar etkileşimleri doğrulanmadı. Spotify çalışmıyordu ve başlatılmadı. Sınırlı ekran dışı çizim ölçümü rapordadır; canlı CPU/RAM garantisi değildir. Değişiklikler commit/push edilmedi.

## 1. Devir durumu ve ilk yapılacak iş

**Phase 2 kodu uygulandı; tam işlevsel doğrulama tamamlanmış değil.** 8 Kasım hazırlığında restore, Debug, Release ve README publish işlemleri başarılıydı. EXE başlatılmıştı; çalışan Spotify olmadığı için görünür widget ve medya etkileşimi doğrulanamamıştı. Güncel çalışma için kökteki `AGENTS.md`, bu belge ve Phase 2 raporu okunmalı; Git durumu yeniden kontrol edilmeli ve Spotify açık izole bir test ortamında aşağıdaki temel davranış matrisi tamamlanmalıdır. Yaşam döngüsü düzeltmelerinin durumu risk tablosunda; canlı doğrulama sınırları ek rapordadır.

Çalışma kökü: `C:\Users\Burhan\Desktop\projects\taskbar`.

| Konu | Hazırlık sonunda durum |
|---|---|
| Kaynak | https://github.com/mechanicwb2-hub/now-playing-taskbar-widget |
| Fork / origin | https://github.com/burhanbty/now-playing-taskbar-widget — public, GitHub tarafından fork olarak doğrulandı |
| upstream | Orijinal depo; fetch ve push URL'si kaynak depodur. Bu depoya push yapılmamalı |
| İncelenen HEAD | `a60265efe16032d29e083ad887cf5d523b9db9d8` — 2026-08-27, troubleshooting belgesi değişikliği |
| Geçmiş | Sığ olmayan klon, HEAD'e kadar 73 commit; orijinal geçmiş ve MIT lisansı korundu |
| Dal | Yerel `development`, başlangıçta `main` / `origin/main` / `upstream/main` ile aynı commit |
| Yayınlama | Hazırlık için commit veya push yapılmadı; `development` uzak depoya gönderilmedi, tracking dalı yok |
| Teslim dosyaları | `AGENTS.md`, `docs/AI_HANDOFF.md`; commit edilmemiş yeni dosyalar |

GitHub CLI `burhanbty` hesabıyla doğrulandı. İlk sandbox denemelerinde ağ/kimlik doğrulama ve `.git` yazma hataları oluştu; izinli yeniden denemeler başarılı oldu. Bunlar proje derleme hatası veya geçersiz hesap olarak yorumlanmamalı.

## 2. Amaç, gerçek özellikler ve README farkları

Uygulama görev çubuğunun boş alanında Spotify parça adı, sanatçı, kapak ve kontrollerini gösteren Windows WPF aracıdır. Gerçek bir deskband veya Explorer içine enjekte edilen bileşen değildir; görev çubuğuna owner ilişkisi kurulan ayrı, şeffaf, kenarlıksız HWND kullanır.

**Kodda mevcut:** Spotify SMTC parça/kapak/durum/timeline okuma; play/pause, önceki/sonraki ve seek; UI Automation üzerinden favoriye ekleme, üç shuffle modu, repeat ve Spotify iç ses düzeyi; UIA başarısızsa shuffle/repeat için SMTC ve ses için Spotify CoreAudio oturumları; açık/koyu sistem temasına uyum; kayan başlık; kapak crossfade; çoklu görev çubuğunda pencere; manuel X konumu; 0.8/1.0/1.1 ölçek; %20–100 opaklık; kontrol görünürlükleri; ilerleme çubuğu; isteğe bağlı Spotify başlatıcı; otomatik gizleme ve fullscreen algılama; başlangıç ve güncelleme menüleri; EN/PT metinler.

**Önemli kaynak/sürüm ayrımı:**

- README artık herhangi bir oynatıcıyı, oyuncu seçme/sabitleme menüsünü ve Windows 10/11 için ayrı yerleşim mantığını anlatıyor. Bu commit'te `MediaService.PickSession()` yalnızca kimliğinde `spotify` geçen ilk oturumu seçiyor. `RefreshTrackAsync()` ayrıca `Spotify` işlemi arıyor; XAML'de oynatıcı seçme menüsü yok. Genel oynatıcı desteği bu kaynak sürümünde mevcut kabul edilemez.
- `SpotifyVolume` yalnızca Spotify PID'lerini ve varsayılan multimedia çıkışındaki oturumları hedefliyor; genel uygulama ses denetleyicisi değil.
- `ApplyResponsiveLayout()` metni kısaltıp düğmeleri kaldırıyor; minimum kapak+metin(+isteğe bağlı play) sığmazsa gizliyor. README'deki yalnız kapak moduna bu kodda rastlanmadı.
- Yerleşimde Windows 11 UIA kimlikleri/sınıfları ve 48 DIP fallback var; Windows 10 için ayrı bir uygulama yolu bulunamadı. Proje TFM'si Windows 10 2004 API tabanını hedeflese de MSIX ve winget minimumu **10.0.22000.0** (Windows 11). Windows 10 uyumluluğu doğrulanmadı.
- README Pro temalar, görselleştirici, senkron şarkı sözleri ve global hotkey anlatıyor. Depoda bunları sağlayan kaynak/modül bulunamadı; Store sürümüyle eşitlik veya Pro koduna erişim varsayılmamalı.
- `TROUBLESHOOTING.md` konum sıfırlamanın monitör seçimini de sıfırladığını söylüyor; `ResetPos_Click()` yalnız manuel konumları temizliyor, monitör seçimini koruyor. Gerçek davranışı koruyun.
- Favori kontrolü bir kaldır/ekle toggle değil: zaten beğenilmişse tıklama işlem yapmaz. Bekleme sırasında 8 saniyelik iyimser yeşil durum bulunur; README'deki her zaman doğrulanmış durum söylemi bunu bütünüyle yansıtmaz.

Bu farklar yerel kaynak incelemesiyle doğrulandı; güncel Store ikilisi karşılaştırılmadı. README'nin uyarısı da kaynak dalının Store'dan geride olabileceğini belirtiyor.

## 3. Ortam, bağımlılıklar ve komutlar

Doğrulanan ortam: Windows 11 Pro x64, **10.0.26300**; .NET SDK **8.0.422**, MSBuild **17.11.48**; .NET 8 Desktop Runtime **8.0.28** mevcut. Git ve GitHub CLI mevcut. Visual Studio kurulumu derleme için gerekli olmadı; yeni SDK, workload veya ürün kurulmadı.

`SpotifyTaskbarWidget.csproj`: `WinExe`, WPF, nullable ve implicit usings açık; `net8.0-windows10.0.19041.0`, `win-x64`, sürüm `1.3.0`, `PublishSingleFile=true`, `SelfContained=false`. ARM64/x86 bu hazırlıkta hedeflenmedi. `global.json` bulunmuyor; başka makinede SDK seçimi yeniden kontrol edilmeli.

**Doğrudan üçüncü taraf PackageReference yok.** WPF, UIAutomation, Windows Runtime medya API'leri, Registry, System.Text.Json, HttpClient ve P/Invoke/COM kullanılıyor. Restore çıktısında SDK tarafından getirilen `Microsoft.NET.ILLink.Tasks 8.0.28` ve `Microsoft.Windows.SDK.NET.Ref 10.0.19041.56` var. Windows API hedefleme paketleri SDK/NuGet ile sağlandı; tam Windows SDK veya C++ workload kurulması gerekmedi. .NET SDK ilk kullanım çıktısı ASP.NET geliştirme sertifikası oluşturulduğunu bildirdi; sertifikaya güven verme komutu çalıştırılmadı.

PowerShell, proje kökünde:

```powershell
git status --short --branch
git remote -v
dotnet --info

# Bu hazırlığın CLI durumunu ve NuGet önbelleğini obj/ altında tutan oturum ayarları:
$env:DOTNET_CLI_HOME = Join-Path $PWD 'obj\cli-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$env:DOTNET_ADD_GLOBAL_TOOLS_TO_PATH = 'false'

dotnet restore SpotifyTaskbarWidget.csproj
dotnet build SpotifyTaskbarWidget.csproj -c Debug --no-restore
dotnet build SpotifyTaskbarWidget.csproj -c Release --no-restore
# README ile aynı publish komutu:
dotnet publish SpotifyTaskbarWidget.csproj -c Release -o publish

# Yalnız test koşulları uygunsa; mevcut kurulu widget ile mutex/ayarlar ortaktır:
& .\publish\SpotifyTaskbarWidget.exe
```

Sandbox restore ilk sefer `NU1301` ile başarısız oldu; ağ erişimli tekrar başarılı. `--no-restore` öncesinde aynı CLI_HOME ile restore gerekir. `obj/` silinirse buradaki araç durumu, önbellek ve doğrulama logları da silinir. Normal ağ izinleri olan Antigravity terminalinde standart dotnet komutları yeterlidir; sandbox için alınan izinler kalıcı proje gereksinimi değildir.

**Çıktılar:**

- Debug: `bin\Debug\net8.0-windows10.0.19041.0\win-x64\SpotifyTaskbarWidget.exe`
- Release: `bin\Release\net8.0-windows10.0.19041.0\win-x64\SpotifyTaskbarWidget.exe`
- Publish: `C:\Users\Burhan\Desktop\projects\taskbar\publish\SpotifyTaskbarWidget.exe`
- Publish EXE: **25,778,648 bayt**, SHA-256 `ABCB49AC6BCEB04F3540869E1EBD3E8CFA3F0378143B855A5C0F2674BAADCEF4`. Yanında `SpotifyTaskbarWidget.pdb` var; uygulama için ayrı DLL dağıtımı gerekmiyor. EXE framework-dependent; hedefte x64 .NET 8 Desktop Runtime gerekir. Bu hash yalnız bu yerel derlemeye aittir.

Inno Setup 6 ve Windows Kits dizinleri mevcut, fakat `ISCC`/`makeappx` PATH üzerinde bulunmadı. Bunlar günlük geliştirme için gerekli değil. `setup.iss` ve `Packaging/build-msix.ps1` incelendi, **çalıştırılmadı**; installer/MSIX derlemesi veya Store yayını bu doğrulamanın parçası değil. Script self-contained, çok dosyalı publish yapıyor ve gerekirse BuildTools indiriyor. Manifest orijinal yayıncının gerçek kimliğini içeriyor; kendi yayınınızda kullanma hakkı varsaymayın, ayrı yayın kararı gerekir. Kurulu sürüme dokunulmadı.

## 4. Doğrulama sonuçları ve sınırlar

| İşlem | Sonuç / kanıt |
|---|---|
| NuGet restore | Başarılı; `obj/preparation/restore.log` |
| Debug build | Başarılı, 0 uyarı / 0 hata; `obj/preparation/build-debug.log` |
| Release build | Başarılı, 0 uyarı / 0 hata; `obj/preparation/build-release.log` |
| README publish | Başarılı; `obj/preparation/publish.log` |
| EXE başlatma | Publish EXE süreci başlatıldı; yolu ve PID'si kontrol edildi, `Responding=true` |
| Görünür GUI | Computer Use `launched app did not expose a targetable window` döndürdü; tekrar pencere araması da boş. Spotify yok ve varsayılan `ShowLauncher=false`; kodla uyumlu, tek başına GUI arızası kanıtı değil |
| Kullanıcı verisi | Başlangıçta ve test sonunda `%APPDATA%\SpotifyTaskbarWidget` yoktu; test ayar/günlük klasörü oluşturmadı. Ayar veya başlangıç menülerine müdahale edilmedi |
| Testin kapanışı | Yalnız bu çalışmada açılan, yolu doğrulanmış test PID'si kapatıldı; mevcut kullanıcı uygulaması kapatılmadı |
| Otomatik testler | xUnit test projesi eklendi (`tests/SpotifyTaskbarWidget.Tests`). Yaşam döngüsü, asenkron iptal, nesil yarışları ve ayar migrasyonu için 9/9 test geçti. |
| Kaynak bütünlüğü | Hazırlık sonunda mevcut takipli dosyalarda fark yok; yalnız iki yeni devir belgesi |

**Kısa boşta ölçümü:** 8 Kasım 23:52 TRT, Spotify yok, varsayılan ayarlar, görünür pencere yok. 10.023 saniyede 0.0469 CPU saniyesi (**tek çekirdeğin %0.47'si**), working set **237.41 MiB**, private memory **155.50 MiB**, **812 handle**. `obj/preparation/startup-smoke.json` kaydı var. Bu kısa, tek koşullu örnek; aktif medya, çoklu monitör veya uzun süreli performans sonucu değil. RAM kullanımı tek başına sızıntı kanıtı değildir. `Responding=true` de gizli süreçte tüm UI akışlarının sağlıklı olduğunu kanıtlamaz.

**Yapılmayan testler:** görünür yerleşim, menüler, gerçek parça/kapak, play/pause/seek, favori/shuffle/repeat/ses, Spotify aç/kapa, farklı oynatıcılar, farklı DPI/monitörler, Windows 10, auto-hide, fullscreen ve Explorer restart. Test için Spotify kurulmadı, hesap açılmadı, kullanıcı verisi veya Windows görev çubuğu ayarı değiştirilmedi. Uygulamanın otomatik güncelleme denetimi 20 saniyeden sonra çalışabilir; güncelleme uygulama komutu kullanılmadı.

## 5. Mimari ve dosya haritası

MVVM/DI framework yok; olay tabanlı WPF ve yoğun code-behind mevcut. `MainWindow.xaml.cs` 2,089 satırla UI, durum, yerleşim, menüler ve yaşam döngüsünü yönetiyor. İlk aşamada toptan framework değişimi gerekmiyor.

```text
App.OnStartup (tek örnek mutex, genel hata işleyici)
  -> MainWindow.SyncToMonitors -> seçili her görev çubuğu için MainWindow
       -> WidgetSettings.Shared <-> JSON; Changed -> tüm pencerelerin UI'si
       -> MediaService -> Windows GSMTC oturumları -> TrackInfo / timeline / kapak
       -> SpotifyUiaService -> Spotify erişilebilirlik ağacı -> ek kontroller
       -> SpotifyVolume -> CoreAudio Spotify oturumları (ses fallback)
       -> TaskbarAnchors -> Explorer UIA -> fiziksel piksel sınırları
       -> Interop -> user32 / shell32 / gdi32; taskbar, HWND, DPI ve hook'lar
       -> L, Diag, PackagedApp, UpdateService, SpotifyActions
```

| Dosya | Sorumluluk ve önemli giriş noktaları |
|---|---|
| `App.xaml`, `App.xaml.cs` | StartupUri yerine `OnStartup`; `SpotifyTaskbarWidget_SingleInstance` mutex; explicit shutdown; DispatcherUnhandledException -> Diag; hiçbir pencere oluşturulamazsa çıkış |
| `MainWindow.xaml` | Şeffaf topmost pencere; Root, ContentPanel, LauncherPanel; vektör ikonlar, iki katmanlı kapak, başlık Canvas, düğmeler, ContextMenu, ProgressTrack, VolumePopup; stiller yerel kaynaklarda |
| `MainWindow.xaml.cs` | `OnLoaded`, `RefreshTrackAsync`, `RefreshTimeline`, kontrol event'leri; `UpdatePosition`, `ApplyResponsiveLayout`, `RefreshAnchors`, `SyncToMonitors`, `OnClosed`; tema, ölçek, marquee, kapak ve auto-hide animasyonu |
| `MediaService.cs` | `TrackInfo` record; `InitializeAsync`, `PickSession`, `Shutdown`; `GetTrackAsync`, `GetThumbnailAsync`, `GetTimeline`; SMTC kontrol metotları |
| `SpotifyUiaService.cs` | `ShuffleMode`, `RepeatMode`; `GetState(expectedTitle)`, `EnsureGroups`, `FindInWindow`, `Invalidate`, `DoWithRetry`; favori/shuffle/repeat/ses; DOM sırası, UIA pattern ve çok dilli isim heuristikleri |
| `SpotifyVolume.cs` | `GetVolume`, `SetVolume`, `ForEachSpotifySession`; el ile tanımlanmış CoreAudio COM interface/vtable'ları; varsayılan ses çıkışı |
| `SpotifyActions.cs` | `OpenSpotifyWindow` restore veya `spotify:` URI; `LikeCurrentTrack` eski odak/klavye fallback'i. Bu ikinci metot mevcut Like_Click yolundan çağrılmıyor |
| `TaskbarAnchors.cs` | `Get(tray)` -> `(Ok, widgetsRight, startLeft, taskButtonsRight)`; WidgetsButton, StartButton ve Taskbar.TaskListButton taraması; tüm ölçüler fiziksel px |
| `Interop.cs` | `Shell_TrayWnd`, `Shell_SecondaryTrayWnd`, `TrayNotifyWnd`; taskbar/monitor rect, appbar auto-hide, fullscreen; SetWindowPos/owner/noactivate; WinEvent hook'ları; bölge kırpma, DPI, odak ve sentetik giriş |
| `WidgetSettings.cs` | Shared model, Load/Save/Changed; JSON normalizasyonu, eski alan migrasyonu, monitör başına ManualX; tmp+rename kaydetme ve SaveLock |
| `L.cs` | Windows CurrentUICulture `pt` ise Portekizce, diğerlerinde İngilizce; Türkçe UI yok. Spotify UIA terim listeleri bundan ayrı |
| `Diag.cs` | `Once` süreç içi anahtar dedup; `Log` ardışık mesajı 30 saniye bastırır, dosya 1 MB'ı geçtiğinde yeniden başlatır |
| `PackagedApp.cs` | Package.Current ile MSIX algılama; Store'da güncellemeyi Store'a, başlangıcı StartupTask'a bırakma |
| `UpdateService.cs` | GitHub release kontrolü, sürüm/asset seçimi, EXE indirme ve geçici CMD ile değiştirme; mevcut repo sabiti eski `mechanicwb2-hub/spotify-taskbar-widget` adını kullanıyor |
| `SpotifyTaskbarWidget.csproj`, `app.manifest`, `app.ico` | Derleme/yayın kimliği, x64, .NET hedefi; PerMonitorV2 DPI farkındalığı; ikon |
| `Packaging/*`, `setup.iss`, `winget/*` | MSIX asset/manifest/script; per-user Inno installer ve isteğe bağlı autostart, runtime indirme; winget metadata/hash. Temel EXE build için gerekli değil |
| `README.md`, `TROUBLESHOOTING.md`, `PRIVACY.md`, `docs/*`, `.github/FUNDING.yml` | Kullanım, sorun giderme, gizlilik, Store/lansman metinleri ve görseller; bazı metinler bu kaynak sürümünden ileride/geride |
| `LICENSE`, `.gitignore` | MIT + MechanicWB telifi ve Spotify marka açıklaması; build/publish/packaging çıktıları ignore edilir |

### Medya ve kontrol akışı

`InitializeAsync()` WinRT manager isteğini ilk denemeden sonra 4/8/16/32/64 saniye beklemelerle yeniden dener. SessionsChanged -> PickSession; seçilen oturumun MediaPropertiesChanged ve PlaybackInfoChanged olayları Changed, TimelinePropertiesChanged ise TimelineChanged üretir. MainWindow bunları Dispatcher'a aktarır; kaçan olaylar için 3 saniyelik parça timer'ı var.

Parça yenileme `_refreshing` ile pencere başına tekilleşir; Spotify süreç kontrolü, 5 saniye bekleme sınırı olan metadata okuma, kısa boş parça toleransı ve kapanış grace süresi kullanır. UIA durum sorgusu normalde en az 5 saniyede bir veya parça/dirty olayında; çağrı bekleme sınırı 8 saniye. UIA grup adını SMTC başlığıyla karşılaştırıp eskimiş favori bilgisini ayırır. Yeni parçada en fazla 12 tur/500 ms aralıklı settle sorgusu bulunur. Bunlar saf polling uygulaması değildir; event ve fallback polling birlikte çalışır.

Kapak metadata'dan ayrı okunur, byte[] -> BitmapImage/OnLoad -> Freeze; UI'da 250 ms fade/crossfade. Başlık gerçek render genişliği ve DPI ile ölçülür; WPF keyframe animasyonu tek sefer veya sürekli kaydırır. Phase 2'de hover/pressed ölçek sıçramaları yerine sabit buton alanı içinde 140 ms VSM opacity geçişleri kullanılır. Windows `ClientAreaAnimation=false` olduğunda yeni geçişler anlık, flyout animasyonu kapalıdır; mevcut kapak/marquee/auto-hide animasyonları değiştirilmedi. Progress her 600 ms son timeline zamanından interpolate edilir; click seek için eski snapshot'lar kısa süre bastırılır.

Görsel renkler `Themes/Dark.xaml` ve `Themes/Light.xaml` kaynaklarındadır. `ApplyTheme` paleti değiştirir; context menu ve volume popup aynı pencere kaynaklarına erişir. Internal sentetik ayar constructor'ı ve `Properties/AssemblyInfo.cs` yalnız izole UI testine olanak sağlar; normal startup hâlâ `WidgetSettings.Shared` kullanır.

UIA, stable controls grubunu cache'ler; başlık/favori grubu her seferinde türetilir. Repeat checkbox, dört çocuk kontrol düğmesi, ilk shuffle düğmesi ve son favori düğmesi gibi yapısal varsayımlar vardır. `Monitor.TryEnter(3s)` kilit beklemesini sınırlar; UIA sağlayıcısındaki tek çağrıyı iptal etmez. Gerçek mouse fallback'i imleci ve odağı kısa süre değiştirebilir; eski durumu finally ile geri yüklemeye çalışır.

### Görev çubuğu, DPI ve monitörler

`OnSourceInitialized()` TOOLWINDOW/NOACTIVATE stillerini ekler. `UpdatePosition()` görev çubuğunu owner yapar; 600 ms timer ve location/foreground hook'larıyla yerleşimi günceller. Fiziksel px ile Win32 taşıma yapar; responsive ölçüler için pencere DPI'sından DIP'ye dönüşür. Manifest PerMonitorV2, XAML layout rounding ve device-pixel snapping açık. Manuel sürükleme WPF koordinatını cihaz dönüşümüyle işler, ayara fiziksel X yazar.

Ortalanmış görev çubuğunda Widgets sağından Start soluna, sola hizalıda app düğmeleri ile tray arasına yerleşir. UIA anchor sorgusu en fazla 5 saniyede bir; başarısız okuma ile gerçekten eksik anchor ayrılır, eski değerler korunur. Tray handle cache'i 2 saniye. Auto-hide iniş/çıkışı 220 ms animasyon ve 10 ms render timer'ıyla izlenir; alt kenardan taşma GDI region ile kırpılır. Popup/menu/sürükleme sırasında görünürlük sabitlenir; fullscreen kontrolü auto-hide kapalıyken kullanılır.

Her seçili görev çubuğunda ayrı MediaService/UIA ve timer'larla bir pencere; ayarlar ortak. İkincil görev çubukları konuma göre sıralanır, fiziksel cihaz kimliğiyle saklanmaz. Monitor çıkarılınca tek yetim pencerenin ana bara fallback'i var; Explorer restart sonrası en fazla 120 saniye bekleyerek yeniden oluşturma tasarlanmış. Karma DPI/gerçek restart bu hazırlıkta denenmedi.

### Ayarlar, hata yönetimi ve ağ

`%APPDATA%\SpotifyTaskbarWidget\settings.json`: AutoPosition/X/MonitorIndex eski alanları, ManualX, Monitors, Scale, Opacity, ShowLauncher/Progress/ScrollTitleOnce ve yedi Show* kontrolü. Eski ayarlar migrasyonu ve opaklık alt sınırı %20 korunmalı. Kaydetme aynı dizinde `.tmp` yazıp rename eder; başarısız save sessizdir ve yine Changed olayı üretir. Tema tercihi depolanan ayrı bir tema alanı değil; Registry'den SystemUsesLightTheme okunur. Manuel serbest Y konumu veya düğme sırası alanı yok.

Dispatcher hataları loglanıp handled edilir; birçok WinRT/UIA/COM hatası null/Unknown/false veya sessiz catch ile yumuşatılır. Bu yaklaşım çöküşü azaltır ama başarısız kontrolü teşhis etmeyi zorlaştırır. Güncelleme kontrolü unpackaged sürümde başlangıçtan 20 saniye sonra ve 6 saatte bir GitHub'a gider; indirme/kendini değiştirme kullanıcı menüsü onayından sonra olur. Store yolunda updater menüsü gizlenir. Autostart değişikliği yalnız menü tıklamasında HKCU Run veya paket StartupTask üzerinden yapılır.

## 6. Teknik riskler — başlangıç incelemesi ve sonraki güncellemeler

Önem: **Yüksek** geliştirmeden önce inceleme, **Orta** ilgili özelliğe dokunmadan önce doğrulama, **Düşük** bakım/izleme. “Kodda doğrulandı” kontrol akışının varlığını belirtir; son kullanıcıda arıza üretildiği anlamına gelmez.

| ID / önem | Kanıt ve durum | Dosya / konum | Önerilen çözüm ve doğrulama |
|---|---|---|---|
| R1 Yüksek | **GİDERİLDİ (Phase 1)**: SessionsChanged adlandırılmış OnSessionsChanged handler'ına bağlandı, Shutdown'da unhook edildi. Disposed bayrağı eklendi; Shutdown'da Changed ve TimelineChanged temizlendi. Idempotent shutdown sağlandı. | `MediaService.cs`, `MainWindow.xaml.cs:2040` | xUnit ile doğrulandı (`LifecycleTests.MediaService_Shutdown_IsIdempotentAndSafe`). |
| R2 Yüksek | **GİDERİLDİ (Phase 1)**: CancellationTokenSource (_initCts) ve _disposed koruması eklendi. Shutdown anında bekleyen retry delay'leri anında iptal ediliyor; kapanan servisin sonradan bağlanması engellendi. | `MediaService.cs` | xUnit ile doğrulandı (`LifecycleTests.MediaService_Shutdown_CancelsPendingInitializationAndMarksDisposed`). |
| R3 Yüksek | **GİDERİLDİ (Phase 1)**: RefreshAnchors içindeki 15s kontrolsüz task watchdog'u kaldırıldı, query bitmeden yeni background task açılması engellendi. RefreshTimeline için `_timelineRunning` guard'ı eklenerek eşzamanlı task çoğalması önlendi. | `MainWindow.xaml.cs:866,1210` | Kod ve test doğrulaması yapıldı; kapanış sonrası arka plan kontrolleri kapatıldı. |
| R4 Orta | **GİDERİLDİ (Phase 1)**: `_trackGeneration` (monoton nesil sayacı) eklendi. Asenkron await sonrası (_closed || generation != _trackGeneration) kontrolleri uygulandı. Hızlı parça değişiminde eski kapak/metadata düşürülüyor. RefreshTimeline, SettleState ve QuickRecheck metodlarına _closed koruması eklendi. | `MainWindow.xaml.cs:917–1105,1210,1323,1335` | xUnit ile doğrulandı (`MediaDataTests.TrackGeneration_MonotonicIncrement`, `TrackStaleDetection_*`). |
| R5 Orta | Tek update loop'u bir pencerenin `_closed` durumuna bağlı. O pencere kaldırılıp başka widget açık kalırsa `_updateCheckStarted` true kalır; loop sona erince yeni kontrol başlatılmayabilir. 6 saatlik delay eski pencereyi süreli tutabilir | `MainWindow.xaml.cs:324–328,1995,2040` | Süreç ömürlü updater veya açık pencereye sahiplik devri, cancel edilebilir delay; ilk pencereyi kaldırıp diğerini bırakma testi |
| R6 Orta | **Kısmen Hafifletildi (Phase 1)**: OnClosed içine `_volPopupWatchdog?.Stop()` ve `_opacitySaveTimer?.Stop()` eklendi. Kalan hook optimizasyonları sonraki fazlarda ele alınacak. | `MainWindow.xaml.cs:2040` | Kod seviyesinde doğrulandı. |
| R7 Orta | UI thread'de Process.GetProcessesByName, Registry okumaları, Win32 sorguları ve tam boy kapak decode var. SMTC await sonrası senkron property çağrıları da UI context'ine dönebilir. UIA'nın çoğu zaten background'da | `MainWindow.xaml.cs:291,410,917,1154,1450`, `MediaService.cs:172` | Önce UI gecikmesini profille; immutable metadata snapshot, küçük cache, DecodePixelWidth gibi sınırlı decode ve uygun background çalışma. Büyük/bozuk kapak ve yavaş sağlayıcı testi |
| R8 Orta | Her monitör aynı medya/UIA işini ve 600 ms/3 s timer'larını tekrarlar; görünmezken fallback polling sürer. Gereksizliğin miktarı ölçülmedi | `MainWindow.xaml.cs:37–39,89–90,291–320` | 1/2/3 monitör, gizli/oynatıyor/paused karşılaştır; paylaşılan medya snapshot'ı ve görünürlük/olay odaklı azaltmayı ancak ölçümden sonra uygula; auto-hide tepkisini koru |
| R9 Orta | UIA Spotify DOM düğme sayısı/sırası ve isim terimlerine bağımlı. Türkçe Smart/Disable terimleri var; PlaylistTerms içinde `çalma listesi` yok. Gerçek Türkçe Spotify etiketleri yakalanmadı | `SpotifyUiaService.cs:24–45,419–536` | Gerçek sürüm/dil erişilebilirlik ağacını doğrula, güvenilir pattern/id tercih et, Unknown durumunu koru. Desktop/web/minimized/Connect ayrımını test et |
| R10 Yüksek | README çoklu oynatıcı ve Win10 iddiaları yerel kodla uyuşmuyor; MSIX/winget minimumu 22000, yerleşim Win11 varsayımları içeriyor | `MediaService.cs:78–94`, `MainWindow.xaml.cs:456`, `TaskbarAnchors.cs:27–52`, `Packaging/AppxManifest.xml`, `winget/*installer.yaml` | Kaynak davranışını temel al; ayrı genel-oynatıcı seçim tasarımı ve Windows sürümü test matrisi. Win10 desteğini tamamlandı olarak yazma |
| R11 Orta | Alt kenar/yatay bar matematiği, 48 DIP ve tray yoksa 220 px fallback; monitor indeksleri topolojiyle değişebilir. Manuel konum yolu app/tray anchor sınırını uygulamıyor | `MainWindow.xaml.cs:390,499–652`, `Interop.cs:47,77` | Üst/yan/çok satırlı/modifiye taskbar ve farklı DPI için kontrollü deneme; desteklenmeyende güvenli fallback. Monitör kimliği migrasyonu ayrı değişiklik olmalı |
| R12 Orta | **Kısmen Hafifletildi (Phase 1)**: RefreshTrackAsync içindeki Process.GetProcessesByName("Spotify") nesneleri açıkça dispose edildi. | `MainWindow.xaml.cs:923` | Kod seviyesinde doğrulandı. |
| R13 Orta | Hata yakalayıcıları komut başarısızlığını sessiz geçiyor; Save başarısız olsa da Changed yayılıyor; Scale için sonlu/pozitif sınır normalizasyonu yok | `WidgetSettings.cs:64–116`, `MediaService.cs` kontrol metotları, `App.xaml.cs:28` | Hassas bilgi içermeyen sınırlı tanılama, save sonucu ve ayar doğrulaması; bozuk JSON/erişim engeli/uygunsuz Scale için izole fixture |
| R14 Orta | Update kaynağı fork'a taşınmıyor; indirilen EXE için boyut+MZ kontrolü var, bağımsız imza/hash doğrulaması yok. Upstream ikilisini geliştirme exe'sinin yerine koyma riski var | `UpdateService.cs:22,76–143`, `MainWindow.xaml.cs:1956` | İlk geliştirme öncesi güncelleme uygulama menüsünü kullanma; ileride ayrı development yayın kanalı, güven doğrulaması ve sabit temp adları/Unicode CMD yolu testleri. Bu aşamada updater değiştirilmedi |
| R15 Düşük | `_clippedWindows` kapanan HWND'ler için açık temizleme sunmuyor; SetWindowRgn başarısızlığında yeni region sahipliği ele alınmıyor | `Interop.cs:116–138` | Başarı/başarısızlıkta GDI sahipliğini ve pencere kapanış temizliğini incele; çok sayıda auto-hide/recreate turunda GDI sayacı ölç |

Build engelleyen doğrulanmış hata yok. Başlatma denemesinde çökme veya ayar yazımı gözlenmedi. Yukarıdaki riskler bu aşamada düzeltilmedi; önem sıralaması ilk testlerden sonra güncellenmeli.

## 7. İstenen özelliklerin uygulanabilirliği ve sırası

Mevcut WPF ile devam edin. Yeni framework/WinUI geçişi veya kapsamlı MVVM dönüşümü önkoşul değildir. Öncelik 0: temel testler + R1–R4 yaşam döngüsü + ölçüm. Aşağıdaki sıra sonraki ürün geliştirme içindir; özellikler bu hazırlıkta uygulanmadı.

| Sıra / özellik | Dosyalar | Yaklaşım / bağımlılık | Çakışma ve kabul ölçütü | Zorluk / risk |
|---|---|---|---|---|
| 1. CPU/RAM optimizasyonu (#10) [Phase 1 kod düzeltmeleri; ölçüm sınırlı] | MainWindow.xaml.cs, MediaService.cs | Yaşam döngüsü/task/process dispose düzeltmeleri mevcut. Eski kısa idle RAM örnekleri optimizasyon veya sızıntı kanıtı değildir. Phase 2 ekran dışı çizim karşılaştırması ek raporda | Uzun süreli aktif medya ve çoklu monitör profili hâlâ gerekli | Canlı ölçüm bekliyor |
| 2. Görev çubuğu uyumu (#11) | Interop.cs, TaskbarAnchors.cs, MainWindow.xaml.cs, app.manifest; gerekirse Packaging/winget metadata | Mevcut px/DIP sınırlarını koru; Windows sürümü/taskbar yetenek tespiti ve güvenli fallback. Standart Win32/UIA yeterli | Fullscreen, owner/restart, tray çakışması, auto-hide ve karma DPI matrisi; sistem ayarını uygulama değiştirmemeli | Yüksek / yüksek |
| 3. Türkçe (#8) | L.cs, MainWindow.xaml(.cs), gerekirse Packaging/AppxManifest.xml; ayrı olarak SpotifyUiaService.cs | L içindeki dil seçimini TR/EN/PT olacak şekilde küçük genişlet; UIA etiket doğrulaması ayrı test. Paket gerekmez | Uzun Türkçe metin, ı/İ/ş/ğ, mevcut EN/PT fallback; XAML sabit metinlerini de denetle. Spotify dili ile widget dilini karıştırma | Düşük–orta / düşük |
| 4. Boyut/opaklık/konum (#6) | WidgetSettings.cs, MainWindow.xaml(.cs), L.cs, Interop.cs | Mevcut Scale, Opacity, ManualX, Monitors üzerine sınırları doğrulanan seçenekler ekle. Yeni bağımlılık yok | Eski JSON/legacy alanları koru; %20 tabanı; manuel Y/serbest boyut taskbar bandına ve DPI'ya çarpabilir | Orta / orta |
| 5. Buton düzenleme (#7) | WidgetSettings.cs, MainWindow.xaml(.cs), L.cs | Görünürlük zaten var; sıra istenirse kararlı kontrol kimlikleri ve eski ayara varsayılan sıra. Sabit paneli küçük adımla düzenle | ApplyResponsiveLayout sabit öncelik ve ölçüler kullanır; kullanıcı sırası ile dar alan önceliğini açık tanımla; play gizlenebilir kalmalı | Orta / orta |
| 6. Fluent modern UI (#1) [UYGULANDI Phase 2] | MainWindow.xaml, MainWindow.xaml.cs, Themes/*.xaml | Segoe UI Variable, merkezi iki palet, yuvarlatılmış kapak, vektör fallback ve tutarlı menü/ses flyout'ı | Debug/Release, 13 xUnit, 100–200% ekran dışı render; canlı Spotify/DPI kabul testi bekliyor | Kod tamam, canlı test açık |
| 7. Hover/geçişler (#4) [UYGULANDI Phase 2] | MainWindow.xaml, MainWindow.xaml.cs | Buton ölçülerini koruyan 140 ms opacity geri bildirimi; azaltılmış harekette anlık durum | VSM/tercih regresyon testi; gerçek mouse ve z-order testi bekliyor | Kod tamam, canlı test açık |
| 8. Dinamik kapak rengi (#3) | MainWindow.xaml.cs (ToBitmap/SetAlbumArt/ApplyThemeIfChanged), MainWindow.xaml, WidgetSettings.cs; küçük renk çıkarıcı dosya | Küçük piksel örneklemiyle background hesap, track-key cache ve kontrast sınırlaması; WPF bitmap API'leri yeterli | SpotifyGreen durum rengini ve aydınlık/koyu kontrastını ezme; eski kapaktan geç sonuç gelmesini nesille engelle; kapaksız fallback | Orta / orta |
| 9. Daha iyi oynatıcı yönetimi (#9) | MediaService.cs, MainWindow.xaml(.cs), WidgetSettings.cs, L.cs, SpotifyVolume.cs; gerekirse küçük provider arayüzü | SMTC session kimliği/yetenekleri, otomatik/Spotify/pinned seçim; Spotify ek kontrollerini özel provider'da tut. Yeni Web API/auth gerekmez | Spotify processAlive filtresi kaldırılmadan genel medya çalışmaz; UIA/volume Spotify'ı yanlışlıkla kontrol etmemeli; yeteneksiz seek/komutları güvenle kapat | Yüksek / yüksek |
| 10. Genişleyen mini oynatıcı (#5) | MainWindow.xaml(.cs), gerekirse ayrı küçük popup pencere, WidgetSettings.cs, Interop.cs, L.cs | Taskbar satırından ayrı genişleyen yüzey; ortak medya snapshot'ı; normalde ayrı servis başlatma | SizeToContent/owner/NOACTIVATE/fullscreen/auto-hide, ekran kenarı ve popup kapanışı; yeniden medya aboneliği üretme | Yüksek / yüksek |
| 11. İsteğe bağlı Liquid Glass (#2) | MainWindow.xaml(.cs), WidgetSettings.cs, Interop.cs veya izole efekt yardımcısı | Önce WPF gradyan/yarı saydam katmanlarla düşük maliyetli benzetim; gerçek backdrop ayrı deney. Efekt kapalıyken temel UI eksiksiz | AllowsTransparency=True ile native backdrop uyumu kanıtlanmadı. Mica cam kırılması değildir; gerçek refraction/blur bütçesi ve compositor bağımlılığı ayrı değerlendirilir; OS desteklemezse solid fallback | Yüksek / yüksek |

Fluent ve Liquid Glass görsel hedeflerdir; Pro kaynaklarını gerektirmez. Native materyal deneyinin Windows App SDK gibi ek dağıtım bağımlılığı doğurabileceği göz önüne alınmalı; yalnız bu efekt uğruna projeye eklenmemeli. [Microsoft'un Mica açıklaması](https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/ui/apply-mica-win32) Mica'yı opak bir materyal olarak tanımlar; istenen cam etkisiyle eşit saymayın. WPF işlerinin Dispatcher üzerinden kısa tutulması için [WPF threading modeli](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/threading-model) esas alınabilir.

## 8. Korunacak davranışlar ve ilk geliştirme paketi

**Kritik koruma listesi:**

1. MIT/MechanicWB telifi, Spotify marka ve bağımsızlık açıklamaları; Git geçmişi.
2. Spotify login/API anahtarı gerektirmeyen yerel medya akışı; mevcut üç shuffle/repeat modu, favoriye yalnız ekleme, volume fallback ve Unknown durumları.
3. Oynatıcı yokken launcher tercihi; kullanıcı istemeden Spotify/medya başlatmama, ses veya favori değiştirmeme.
4. Odak çalmayan ana pencere, menü/popup/tooltip sıralaması; gerçek click fallback'inin kullanıcı imlecini/odağını geri getirmesi.
5. DPI farkındalığı, fiziksel piksel yerleşimi, monitör başına manuel konum ve ortak görünüm ayarları; sıfırlamada monitör seçimini koruma.
6. Dar alanda düğme azaltma/gizlenme, fullscreen ve auto-hide; interaction sırasında popup/sürüklemeyi yarıda kesmeme.
7. Ayar migrasyonu, atomik dosya değiştirme, opaklık alt sınırı; mevcut ayar dosyalarını test için sıfırlamama.
8. Tek örnek mutex, explicit exit ile Explorer kaynaklı kapanışın ayrımı; Store/unpackaged güncelleme ve autostart farkı.

**Antigravity'nin ilk paketi:**

1. `git status`, HEAD ve remotes'u yeniden doğrula. Bu iki belgenin commit edilmemiş kullanıcı teslimatı olduğunu bil; silme veya üzerine checkout yapma. Geliştirme `development` üzerinde küçük değişikliklerle ilerlesin.
2. Restore + Debug + Release baseline'ını yeniden al. GUI testini ayrı Windows kullanıcı profilinde/VM'de veya açıkça ayrılmış test ortamında yap. **Yalnız APPDATA ortam değişkeni vermek bu uygulamayı izole etmiyor:** hazırlıkta `[Environment]::GetFolderPath('ApplicationData')` gerçek Roaming dizinini döndürdü. Uygulamanın kendi veri-kökü override'ı yok.
3. Spotify açıkken aşağıdaki matrisi tamamla; ekran görüntüsü ve ölçüm koşullarını kaydet. Mevcut kullanıcının settings/kurulum/autostart'ına dokunma; test gerekçesiyle Explorer'ı aktif kullanıcı oturumunda yeniden başlatma.
4. Önce R1/R2 için manager aboneliği ve init/shutdown sınırını, ardından R3/R4 için eski sonuç/iş birikmesini ele al. Her değişiklik ayrı, test edilebilir olsun; amaçlanan davranışı aynı tut. Küçük sentetik yaşam döngüsü testleri eklemek uygundur; test framework geçişi gerekmez.
5. Debug ve Release derle, ilgili regression senaryosunu tekrar çalıştır; ancak sonuçlar kayıtlıysa sıradaki pakete geç. Performans ölçümünden sonra ürün özellikleri tablosundaki sırayı izle. Kullanıcı istemeden commit/push/release/kurulum yapma.

### Tamamlanacak temel regresyon matrisi

| Senaryo | Beklenen kontrol | Bu hazırlıktaki durum |
|---|---|---|
| Spotify yok, varsayılan ayar | Sessizce gizli kalır; süreç/ayar değişimi izlenir | Sınırlı süreç testi yapıldı, görünür GUI yok |
| Spotify açık/playing/paused/minimized | Başlık/kapak/doğru play durumu; stale favori yerine Unknown | Yapılmadı |
| Play/prev/next/seek; hızlı parça değişimi | Komut doğru oturuma; eski timeline geri sıçramaz | Yapılmadı |
| Favori/shuffle/repeat/ses | Mevcut davranış; UIA/SMTC/CoreAudio fallback; odak/imleç geri gelir | Yapılmadı |
| Buton görünürlüğü, ölçek, opaklık, scroll-once | Tek/çok pencerede ayar paylaşımı ve restart sonrası kalıcılık | Yapılmadı |
| Ortalı/sola hizalı, tray kalabalık, weather kapalı | App/tray üstüne taşmama; kapasiteye göre küçülme/gizlenme | Yapılmadı |
| Auto-hide/fullscreen; açık menu/tooltip/volume | Gizlenme, geri geliş, z-order ve interaction tutarlılığı | Yapılmadı |
| 1/2+ monitör; 100/125/150/200%; negatif koordinatlar | Doğru bara yerleşim; her monitorün konumu bağımsız | Yapılmadı |
| Monitor çıkar/ekle ve Explorer restart (izole ortam) | Pencere/hook çoğalmadan geri dönüş | Yapılmadı |
| 30 dk idle/oynatma + tekrar aç/kapa | CPU/private bytes/handle/GDI eğilimi; log büyümesi sınırlı | Yalnız 10 s idle örneği alındı |
| Windows 10 / web player / Apple Music | Şimdiki sınırlamaları kaydet; destek varmış gibi pass verme | Yapılmadı, kaynakta genel oynatıcı desteği yok |
| Bozuk/eski JSON, yazılamayan test dizini | Çökmeyen yükleme ve migrasyon, sessiz veri kaybının analizi | Yapılmadı |

## 9. Açık işler ve sınır

Kullanıcının fork/branch/araç hazırlığı için tamamlaması gereken kimlik doğrulama veya kurulum adımı kalmadı. Antigravity için yerel klasör açılabilir. Tam GUI/medya doğrulaması için Spotify'ın çalıştığı bir test oturumu gerekir; gerekiyorsa bunun kurulumu/girişi kullanıcı tarafından yapılmalıdır. Devir belgelerini başka makineden kullanmak istenirse sonradan commit/push gerekir; bu görevde gereksiz yayın yapılmadı.

Bu belge Store ikilisinin tersine mühendisliği, Pro özelliklerinin uygulanması, yeni UI geliştirmesi, performans garantisi veya dağıtım onayı değildir. Sonraki çalışma, doğrulanmamış alanları test ederek mevcut özellikleri korumalıdır.
