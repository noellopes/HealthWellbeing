
using System.ComponentModel.DataAnnotations;

public class EventType {
    public int EventTypeId { get; set; }

    [Required, StringLength(200)]
    public string EventTypeName { get; set; }

    public string? EventTypeDescription { get; set; }

}
