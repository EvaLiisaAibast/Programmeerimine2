namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Messages that the public API returns to the end user.
    /// </summary>
    public static class PublicMessages
    {
        public const string UserNotIdentified = "Kasutajat ei õnnestunud tuvastada. Palun logi uuesti sisse.";

        public const string CustomerNotFound = "Sinu kontoga ei ole seotud kliendiprofiili. Palun registreeru uuesti.";

        public const string RecordNotFound = "Kirjet ei leitud.";

        public const string AccountBlocked = "Sinu konto on blokeeritud. Ostmiseks võta ühendust klienditeenindusega.";
    }
}
