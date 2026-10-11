
namespace EnterpriseReportSystem.Models.Entities.Structure;

public class FacilityLog
{
    public int Id { get; set; } // Числов първичен ключ
    public DateTime LogDate { get; set; } = DateTime.Now; // Счетоводна дата на промяната

    // Вид на събитието: CREATED, RENAMED, MOVED, STATUS_CHANGED
    public string EventType { get; set; } = string.Empty;

    public string OldValue { get; set; } = string.Empty; // Старо състояние
    public string NewValue { get; set; } = string.Empty; // Ново състояние

    // Връзка към обекта, за който се отнася историята
    public int FacilityId { get; set; }
    public Facility Facility { get; set; } = null!;
}
