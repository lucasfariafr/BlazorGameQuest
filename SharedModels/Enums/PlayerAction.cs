using System.ComponentModel.DataAnnotations;

namespace SharedModels.Enums;

public enum PlayerAction
{
    [Display(Name = "Combattre")]
    Fight = 0,

    [Display(Name = "Fuir")]
    RunAway = 1,

    [Display(Name = "Chercher")]
    Search = 2,

    [Display(Name = "Ouvrir")]
    Open = 3,

    [Display(Name = "Ignorer")]
    Ignore = 4
}
