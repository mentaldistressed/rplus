namespace DocumentManager.Models
{
    public class Agreement
    {
        public string? Id { get; set; }

        public int? applId { get; set; }

        public int typeId { get; set; }

        public string? Country { get; set; }

        public string? Fio { get; set; }

        public string? fioTP { get; set; }

        public string? NickName { get; set; }

        public string? AgreementDate { get; set; }

        public string? documentDate { get; set; }

        public PassData? PassData { get; set; }

        public List<RoyalityData>? RoyalityDatas { get; set; }

        public List<Music>? Musics { get; set; }
    }
}
