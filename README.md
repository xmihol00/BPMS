## Autor
jméno: **David Mihola** \
email: xmihol00@fit.vutbr.cz \
datum: 11. 5. 2022

## Obsah
* ``BPMS/``: Adresář obsahuje jádro systému - HTTP server, třídy definující HTTP koncové body systému - řadiče, soubory generující HTML obsah - pohledy, soubory s CSS styly, soubory s JavaScript kódem a konfigurační soubor systému.
* ``BPMS_BL/``: Adresář obsahuje třídy, které implementují převážnou část obchodní logiky systému - fasády, a k nim další pomocné třídy.
* ``BPMS_Common/``: Adresář obsahuje výčtové typy a jiné statické třídy, které je nutné odkazovat ze všech částí systému.
* ``BPMS_DAL/``: Adresář obsahuje třídy, které pomocí ORM definují schéma databáze, a třídy implementující komunikaci s databází - repozitáře.
* ``BPMS_DTOs/``: Adresář obsahuje třídy definující objekty používané k mapování získaných dat z databáze pro jejich následné zobrazení v pohledech.
* ``Models/``: Adresář obsahuje BPMN modely, které lze nahrát do systému, a vyzkoušet si tak jeho funkce. 
* ``Thesis/``: Adresář obsahuje zdrojové soubory bakalářské práce a z nich vygenerovaný soubor PDF.
* ``BPMS.sln``: Soubor pro otevření projektu v programu *Visual Studio 2022*.
* ``LICENCE``: Soubor s licencí obsahu na paměťovém médiu.
* ``README.md``: Soubor s důležitými informacemi.

## Návod pro lokální spuštění
Pro lokální spuštění je nutné mít nainstalované produkty .NET SDK 6.0 (odkaz pro stažení: https://dotnet.microsoft.com/en-us/download), SQL Server (odkaz pro stažení: https://www.microsoft.com/en-us/sql-server/sql-server-downloads) databázi a nástroj LibMan (návod pro instalaci: https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli?view=aspnetcore-6.0). Následuje návod pro lokální spuštění na operačním systému Windows a operačních systémech linuxového typu.

### Spuštění na Windows
1. Pokud nechcete používat *Visual Studio 2022*, postupujte dle návodu pro Linux.
2. Otevřete projekt ve *Visual Studio 2022* pomocí souboru ``BPMS.sln``.
3. Zkontrolujte, že Vaše instalace SQL server podporuje ``LocalDB``.
4. Pokud ne, otevřete soubor ``BPMS/appsettings.json`` a v záznamu ``"DB"`` nahraďte připojovací řetězec k databázi záznamem identifikující prázdnou databázi, kterou chcete použít. Jinak tento krok můžete přeskočit, databáze se vytvoří sama.
5. V souboru ``BPMS/appsettings.json`` nahraďte obsah záznamu ``"FileStore"`` adresářem s adekvátními právy na zápis, do kterého mají být ukládány soubory nahrané do systému.
6. Stáhněte klientské knihovny vybráním možnosti Restore Client-Side Libraries po kliknutí pravým tlačítkem myši na soubor ``BPMS/libman.json``.
7. V souboru ``BPMS/Properties/launchSettings.json`` případně změňte URL nebo pouze port, na kterém aplikace poběží.
8. Spusťte ladění ve *Visual Studio 2022*.
9. Na přihlašovací stránce zvolte První přihlášení a vytvořte si heslo pro účet s přihlašovacím jménem admin. Pomocí tohoto účtu lze následně vytvořit další uživatelské účty.
10. V modulu Systémy editujte URL systému Tento systém tak, aby odpovídala URL, na které je systém dostupný.

### Spuštění na Linux
1. Otevřete soubor ``BPMS/appsettings.json`` a v záznamu ``"DB"`` nahraďte připojovací řetězec k databázi záznamem identifikující prázdnou databázi, kterou chcete použít.
2. V souboru ``BPMS/appsettings.json`` nahraďte obsah záznamu ``"FileStore"`` adresářem s adekvátními právy na zápis, do kterého mají být ukládány soubory nahrané do systému.
3. Navigujte do adresáře BPMS a spusťte zde příkaz libman restore, který stáhne klientské knihovny.
4. V souboru ``BPMS/Properties/launchSettings.json`` případně změňte URL nebo port, na kterém aplikace poběží.
5. V adresáři BPMS spusťte systém použitím příkazu dotnet run.
6. Navigujte na použitou URL ve webovém prohlížeči.
7. Na přihlašovací stránce zvolte První přihlášení a vytvořte si heslo pro účet s přihlašovacím jménem admin. Pomocí tohoto účtu lze následně vytvořit další uživatelské účty.
8. V modulu Systémy editujte URL systému Tento systém tak, aby odpovídala URL, na které je systém dostupný.