using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.Data.Entities;

[Table("accounts")]
[Index(nameof(CurrencyId), nameof(UserId), IsUnique = true)]
[Index(nameof(CurrencyId))]
[Index(nameof(UserId))]
public class AccountEntity
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Column("sum")]
    public decimal Sum { get; set; }
    
    [Column("currency_id")]
    public long CurrencyId { get; set; }
    
    [ForeignKey(nameof(CurrencyId))]
    public CurrencyEntity? Currency { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public UserEntity? User { get; set; }
}