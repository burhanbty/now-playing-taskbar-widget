# Antigravity ve diğer kod ajanları için teknik kurallar

## Başlangıç

- Önce `docs/AI_HANDOFF.md` dosyasını oku. README ile bu commit'in gerçek özellikleri arasında farklar var; kaynak kod ve tekrar edilebilir doğrulama esas alınmalı.
- İşe başlamadan `git status --short --branch`, `git remote -v` ve `git log -1` kontrol et. Kullanıcı değişikliklerini, yeni belgelerini veya mevcut dallarını ezme; `reset --hard`, zorla checkout/clean veya force push yapma.
- Geliştirme için yerel `development` hazırlandı. `origin` kullanıcının fork'u, `upstream` orijinal depodur. Upstream'e push yapma. Kullanıcı istemeden commit, push, merge, release, Store yayını veya installer çalıştırma.

## Değişiklik ilkeleri

- **Mevcut özellikleri korumak zorunludur.** Yeni özellik uğruna medya kontrollerini, DPI/monitor yerleşimini, dar alan davranışını, auto-hide/fullscreen, odak yönetimi veya ayar migrasyonunu kaldırma.
- **Değişiklikleri küçük, tek amaçlı ve test edilebilir tut.** Aynı pakette görsel yenileme, medya seçimi ve yaşam döngüsü refactor'u yapma. Gereksiz refactoring, yeni framework veya bağımlılık ekleme; mevcut WPF/.NET 8 temeliyle başla.
- **Her önemli değişiklikten sonra Debug ve Release derlemesi zorunludur.** İlgili davranış testlerini de çalıştır. Başarısız derlemeyi veya yapılmayan GUI testini başarılı gibi raporlama.
- Hazırlık görevi sırasında yeni özellik/kaynak/UI değişikliği yapılmamalıdır. Sonraki geliştirme kapsamını kullanıcının o oturumdaki talebi belirler; bu dosya tüm yol haritasını kendiliğinden uygulama yetkisi vermez.
- MIT lisansı, MechanicWB telifi, Spotify marka/bağımsızlık bildirimleri ve orijinal geçmiş korunmalı. Store Pro özelliklerinin kaynak kodunun burada bulunduğunu varsayma.

## Teknik sınırlar

- Ana pencere odak çalmamalı; popup, tooltip ve context menu z-order davranışını koru. Kullanıcı istemeden Spotify'ı başlatma veya medya, ses ve favori durumunu değiştirme.
- Yerleşim için fiziksel px ile WPF DIP ayrımını koru; PerMonitorV2, karma DPI, negatif ekran koordinatları ve monitör başına ManualX birlikte test edilmeli.
- Ayarlar Shared/Changed ile pencereler arasında paylaşılır. Eski JSON alanları/migrasyon, atomik save ve %20 opaklık alt sınırı korunmalı. Yeni ayarlara geriye uyumlu varsayılanlar ve doğrulama ekle.
- WinRT/UIA callback'lerinden WPF nesnelerine doğrudan erişme; Dispatcher üzerinden uygula. UI thread'ini senkron bekleme, UIA/COM taraması veya pahalı decode ile bloke etme.
- Async işler pencere/servis kapanışını ve oturum/parça değişimini gözetmeli. Timeout'un alttaki işi iptal ettiğini varsayma; sınırsız Task.Run/polling üretme.
- Event aboneliklerini açık bir sahiplikle çöz; timer, hook, Process/stream ve COM yaşam döngüsünü denetle. Paylaşılan COM nesnelerine körlemesine FinalReleaseComObject uygulama.
- Spotify UIA isim/DOM varsayımlarını gerçek sürüm ve dille doğrula. Unknown/freshness ayrımını koru; genel oynatıcı desteğini uygulamadan varmış gibi raporlama.
- Performans değişiklikleri ölçüme dayanmalı. Koşul, süre, monitor sayısı ve CPU normalizasyonunu kaydet; tek RAM örneğini sızıntı kanıtı sayma.
- Store/unpackaged startup ve update yollarını ayır. Mevcut updater orijinal yayınları hedefler; geliştirme EXE'sinde güncelleme uygulama menüsünü kullanma. Yayıncı kimliği veya güncelleme kaynağı değişikliği ayrı kapsam olmalı.

## Kullanıcı verisi ve testler

- Mevcut `%APPDATA%\SpotifyTaskbarWidget`, kurulu sürüm, Registry autostart veya Windows görev çubuğu ayarlarını test için değiştirme/sıfırlama. Gerçek ayar dosyasını fixture yapma; kişisel veriyi log/commit'e ekleme.
- GUI ve kalıcılık testlerini ayrı profil/VM veya gerçekten izole test ortamında yap. Yalnız APPDATA ortam değişkeni bu uygulamada izolasyon sağlamaz; kod Environment.GetFolderPath kullanır.
- Aktif kullanıcı oturumunda Explorer restart, monitor/ölçek/taskbar ayarı değişikliği gibi testler yapma; izole test ortamı kullan. Test sürecini kapatırken yalnız kendi başlattığın ve yolunu/PID'sini doğruladığın süreci kapat.
- Depoda başlangıçta test projesi/CI yok. Gerektiğinde yaşam döngüsü, session seçimi ve ayar migrasyonu gibi anlamlı regresyon testleri ekle; geçen test sayısı uydurma. GUI engelini ve yapılmayan testleri açıkça yaz.

## Derleme ve teslim

Proje kökünde PowerShell:

```powershell
dotnet restore SpotifyTaskbarWidget.csproj
dotnet build SpotifyTaskbarWidget.csproj -c Debug --no-restore
dotnet build SpotifyTaskbarWidget.csproj -c Release --no-restore
dotnet publish SpotifyTaskbarWidget.csproj -c Release -o publish
```

Hazırlıkta kullanılan `obj/cli-home` önbelleği için oturum değişkenleri ve tekrar üretim ayrıntıları devir belgesindedir. `bin/`, `obj/`, `publish/` ve packaging çıktıları commit edilmemeli. `.gitignore` kurallarını koru.

Her teslimde neyin değiştiğini, nedenini, derleme/test sonucunu ve kalan sınırlamaları belirt. Yeni bulguları doğrulanmış hata / kodda görülen risk / test edilmemiş varsayım olarak ayır; anlamlı değişikliklerden sonra `docs/AI_HANDOFF.md` içindeki ilgili davranış ve sonuçları güncelle.
