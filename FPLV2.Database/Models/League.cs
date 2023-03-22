using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLV2.Database.Models;

public class League : BaseModel
{
    public int LeagueId { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; }
    public int StartEvent { get; set; }
}
