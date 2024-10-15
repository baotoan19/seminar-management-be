using Seminar.CORE.Base;

namespace Seminar.DOMAIN.Entitys;

public class Review_Board_Member : BaseEntity
{
    public int ReviewerId { get; set; }
    public virtual Reviewer Reviewer { get; set; }
    public int ReviewCommitteeId { get; set; }
    public virtual Review_Committee ReviewCommittee { get; set; }
    public string Description { get; set; }
    public bool IsStatus { get; set; }
}
