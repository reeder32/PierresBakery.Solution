using System.Collections.Generic;

namespace PierresBakery.Models
{
  public class Treat
  {
    public Treat()
    {
      FlavorTreats = new HashSet<FlavorTreat>();
    }
    public int TreatId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<FlavorTreat> FlavorTreats { get;}
  }
}