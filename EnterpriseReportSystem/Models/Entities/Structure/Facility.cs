using EnterpriseReportSystem.Models.Entities.Location;
namespace EnterpriseReportSystem.Models.Entities.Structure;

public class Facility
{
    public int Id { get; set; } // Числов първичен ключ (Identity)
    public string FacilityCode { get; set; } = string.Empty; // Твоят уникален код на обекта
    public string Name { get; set; } = string.Empty; // напр. Кравеферма Северен сектор
    public string PostalCode { get; set; } = string.Empty; // Пощенски код на терена

    
    public string Address { get; set; } = string.Empty; // НОВОТО СВОЙСТВО ЗА ТОЧЕН АДРЕС / МЕСТНОСТ

    // Жизнен цикъл и свободен текст за одит
    public bool IsActive { get; set; } = true; // Чекбокс за активност
    public string Notes { get; set; } = string.Empty; // Свободни бележки
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    // Външен ключ към Населеното място, където ФИЗИЧЕСКИ се намира фермата
    public int TownOrVillageId { get; set; }
    public TownOrVillage TownOrVillage { get; set; } = null!;

    // Външен ключ към собственика на обекта (Фирма или Човек)
    public int CompanyOrPersonId { get; set; }
    public CompanyOrPerson CompanyOrPerson { get; set; } = null!;

    // Хронологичен дневник на промените за този конкретен обект
    public ICollection<FacilityLog> FacilityLogs { get; set; } = new List<FacilityLog>();
}
