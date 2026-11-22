# 06 · Git / GitHub + CI

> Durum: depo `git init` ile açıldı (`main`), henüz commit atılmadı. `.github/workflows/ci.yml` yazıldı ama **GitHub'a push edilmediği için henüz çalışmadı/doğrulanmadı**.

## Git mantığı (3 alan)
`çalışma klasörü` —`git add`→ `staging` —`git commit`→ `yerel depo` —`git push`→ `GitHub`
Commit = dosyaların anlık görüntüsü + mesaj + üst commit işaretçisi. Branch = bir commit'i gösteren hareketli etiket.

## İlk kez bu projeyi GitHub'a koymak
```powershell
cd C:\Users\emref\source\repos\EMagaza
git add .
git status                                   # NELER gidiyor diye BAK (.db, bin/obj gitmemeli → .gitignore)
git commit -m "feat: EMağaza ilk sürüm"
# GitHub'da boş (README'siz) bir repo aç, sonra:
git remote add origin https://github.com/<kullanici>/EMagaza.git
git push -u origin main
```
`.gitignore` neden önemli: `bin/`, `obj/`, `*.db` (veri), gizli anahtarlar repoya girmez. Bir kez commit'e girerse geçmişte kalır.

## Günlük akış (mülakatta anlatılacak)
```powershell
git switch -c feature/yorumlar     # dal aç
# kod yaz...
git add -p                          # parça parça seç
git commit -m "feat: ürün yorumları"
git push -u origin feature/yorumlar
# GitHub'da Pull Request aç → CI otomatik çalışır → review → merge
git switch main; git pull
```
Merge vs rebase: merge geçmişi olduğu gibi korur (merge commit), rebase commit'leri yeniden yazıp çizgisel yapar. **Paylaşılmış dalı asla rebase etme.**
Geri alma: `git restore <dosya>` (kaydedilmemiş), `git revert <sha>` (güvenli, yeni commit), `git reset --hard` (tehlikeli, yerel işi siler).
Commit mesajı: `feat:`, `fix:`, `docs:`, `test:` (Conventional Commits).

## CI = Continuous Integration → `.github/workflows/ci.yml`
Her push/PR'da sunucu: kodu çek → derle → test et. Kırık kod `main`'e girmeden yakalanır.
```yaml
on: { push: { branches: [main] }, pull_request: }   # tetikleyici
jobs:
  build-test:
    runs-on: ubuntu-latest                          # GitHub'ın verdiği geçici makine
    steps:
      - uses: actions/checkout@v4                   # kodu indir
      - uses: actions/setup-dotnet@v4               # .NET 9 kur
      - run: dotnet restore / build / test          # bizim komutlarımız, yerelde çalıştırdıklarımızın aynısı
  docker:
    needs: build-test                               # sıra: testler geçmeden imaj derlenmez
```
**CI vs CD:** CI = derle+test. CD = (Continuous Delivery/Deployment) başarılıysa imajı kayıt defterine (ghcr.io) gönder ve sunucuya yayınla. Bizde CD adımı henüz yok; eklenecek.
Sır yönetimi: şifre/anahtar YAML'a yazılmaz → GitHub *Settings → Secrets* ve `${{ secrets.ADI }}`.

## Mülakat soruları
Fetch vs pull? Merge vs rebase? Revert vs reset? `.gitignore` sonradan eklenince izlenen dosya neden hâlâ takipte (`git rm --cached`)? CI/CD farkı? Workflow'da `needs` ne işe yarar? Secret'lar nasıl saklanır?
