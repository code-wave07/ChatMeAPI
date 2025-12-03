namespace ChatMe.Models.DTOs.Responses;

public class ConversationResponse
{
    public string ConversationId { get; set; }
    public string ConversationName { get; set; } // "Marketing Team" or "John Doe"
    public string? LastMessage { get; set; }
    public DateTime LastMessageTime { get; set; }
    public bool IsGroup { get; set; }
    public string? OtherUserId { get; set; } // If private, store the other person's ID here
}