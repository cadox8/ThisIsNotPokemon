using System.Collections.Generic;
using pokemon;
using UnityEngine;

namespace Data {
    public class ConditionsDB {

        public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
            new Dictionary<ConditionID, Condition>()
            {
                {
                    ConditionID.psn,
                    new Condition()
                    {
                        Name = "Veneno",
                        Color = new Color(193, 51, 255),
                        StartMessage = "ha sido enveneado!",
                        OnAfterTurn = pokemon =>
                        {
                            pokemon.UpdateHP(pokemon.MaxHp / 8);
                            pokemon.StatusChanges.Enqueue($"El veneno ha hecho daño a {pokemon.Base.Name}");
                        }
                    }
                },
                {
                    ConditionID.brn,
                    new Condition()
                    {
                        Name = "Quemado",
                        Color = new Color(255, 87, 51),
                        StartMessage = "ha sido quemado!",
                        OnAfterTurn = pokemon =>
                        {
                            pokemon.UpdateHP(pokemon.MaxHp / 16);
                            pokemon.StatusChanges.Enqueue($"Las llamas han hecho daño a {pokemon.Base.Name}");
                        }
                    }
                },
                {
                    ConditionID.par,
                    new Condition()
                    {
                        Name = "Paralizado",
                        Color = new Color(255, 215, 51),
                        StartMessage = "ha sido paralizado! Quizás no se pueda mover",
                        OnBeforeMove = pokemon =>
                        {
                            if (Random.Range(1, 5) == 1)
                            {
                                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} está paralizado y no se puede mover");
                                return false;
                            }

                            return true;
                        }
                    }
                },
                {
                    ConditionID.frz,
                    new Condition()
                    {
                        Name = "Congelado",
                        Color = new Color(149, 255, 249),
                        StartMessage = "ha sido congelado!",
                        OnBeforeMove = pokemon =>
                        {
                            if (Random.Range(1, 5) == 1)
                            {
                                pokemon.CureStatus();
                                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} ya no está congelado");
                                return true;
                            }

                            return false;
                        }
                    }
                },
                {
                    ConditionID.slp,
                    new Condition()
                    {
                        Name = "Dormido",
                        Color = new Color(178, 178, 178),
                        StartMessage = "ha sido dormido!",
                        OnStart = pokemon => pokemon.StatusTime = Random.Range(1, 4),
                        OnBeforeMove = pokemon =>
                        {
                            if (pokemon.StatusTime <= 0)
                            {
                                pokemon.CureStatus();
                                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} se despertó");
                                return true;
                            }
                            
                            pokemon.StatusTime--;
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} está dormido");
                            return false;
                        }
                    }
                },
                {
                ConditionID.con,
                new Condition()
                {
                    Name = "Confundido",
                    Color = new Color(178, 178, 178),
                    StartMessage = "está confuso!",
                    OnStart = pokemon => pokemon.VolatileStatusTime = Random.Range(1, 4),
                    OnBeforeMove = pokemon =>
                    {
                        if (pokemon.VolatileStatusTime <= 0)
                        {
                            pokemon.CureVolatileStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} ya no está confuso");
                            return true;
                        }
                            
                        pokemon.VolatileStatusTime--;

                        if (Random.Range(1, 3) == 1) return true;
                        
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} está confuso");
                        pokemon.UpdateHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"Está tan confuso que se ha autolesionado");
                        return false;
                    }
                }
            }
            };
        
        public static void Init()
        {
            foreach (var kv in Conditions) kv.Value.Id = kv.Key;
        }
    }

    public enum ConditionID {
        none, psn, brn, slp, par, frz, con
    }
}