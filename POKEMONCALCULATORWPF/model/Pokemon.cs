﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static POKEMONCALCULATORWPF.model.MainPokemonCalc;

namespace POKEMONCALCULATORWPF.model
{
    public class Pokemon
    {
        public static string CHEMIN_DOSSIER = "jsonstock";

        private string name;
        private int id;
        private Sprite sprites;
        private PokemonSpecies pSpecies;
        private List<Types> types;
        private Stat[] stats;
        private List<Abilities> abilities;

        public string Name { get => name; set => name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower(); }
        public int Id { get => id; set => id = value; }
        public Sprite Sprites { get => sprites; set => sprites = value; }
        public PokemonSpecies PSpecies { get => pSpecies; set => pSpecies = value; }
        public List<Types> Types { get => types; set => types = value; }
        public Stat[] Stats { get => stats; set => stats = value; }


        // WPF
        private string frName, typeChartResume;
        public string FrName { get => frName; set => frName = value; }
        public string TypeChartResume { get => typeChartResume; set => typeChartResume = value; }
        public List<TypeP> ResistancesX2 { get => resistancesX2; set => resistancesX2 = value; }
        public List<TypeP> ResistancesX4 { get => resistancesX4; set => resistancesX4 = value; }
        public List<TypeP> FaiblessesX2 { get => faiblessesX2; set => faiblessesX2 = value; }
        public List<TypeP> FaiblessesX4 { get => faiblessesX4; set => faiblessesX4 = value; }
        public List<TypeP> Immunites { get => immunites; set => immunites = value; }
        public List<Abilities> Abilities { get => abilities; set => abilities = value; }

        private List<TypeP> resistancesX2, resistancesX4, faiblessesX2, faiblessesX4, immunites;

        public Pokemon()
        {}

        public async Task SetSpecies()
        {
            pSpecies = await GetPokemonSpeciesById(Id);
        }

        public bool HasTwoTypes()
        {
            return this.Types.Count == 2 ? true : false;
        }

        private string ToFrString()
        {
            string frName = PSpecies.Names.Find(x => x.Language.Name == "Fr").Name;
            return frName;
        }

        public void SetFrName() { FrName = ToFrString();}

        // Méthode pour récupérer l'espèce du pokemon à partir de son id
        public static async Task<PokemonSpecies> GetPokemonSpeciesById(int id) //url de pokemonspecies à mettre à la place
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    PokemonSpecies especePokemon = JsonConvert.DeserializeObject<PokemonSpecies>(contenu);
                    return especePokemon;
                }
                else
                {
                    Console.WriteLine($"Erreur lors de la récupération du Pokémon avec l'ID {id}.");
                    return null;
                }
            }
        }

        public void Serialize()
        {
            string json = JsonConvert.SerializeObject(this);
            string cheminFichier = $"jsonstock/{this.Name}.json";


            if (!Directory.Exists(CHEMIN_DOSSIER))
            {
                Directory.CreateDirectory(CHEMIN_DOSSIER);
            }
            File.WriteAllText(cheminFichier, json);
        }

        public override string ToString()
        {
            return Name;
        }

        public void SetDoubleTypesSpecifiations()
        {
            ResistancesX2 = new List<TypeP>();
            ResistancesX4 = new List<TypeP>();
            FaiblessesX2 = new List<TypeP>();
            FaiblessesX4 = new List<TypeP>();
            Immunites = new List<TypeP>();

            Dictionary<TypeP, double> resistances, faiblesses;

            if (HasTwoTypes())
            {
                resistances = MainPokemonCalc.GetResistances(Types[0].Type.Name, Types[1].Type.Name);
                faiblesses = MainPokemonCalc.GetFaiblesses(Types[0].Type.Name, Types[1].Type.Name);
            }
            else
            {
                resistances = MainPokemonCalc.GetResistances(Types[0].Type.Name, "");
                faiblesses = MainPokemonCalc.GetFaiblesses(Types[0].Type.Name, "");
            }

            foreach (KeyValuePair <TypeP,double> kvp in resistances)
            {
                if (kvp.Value == 0.5)
                {
                    ResistancesX2.Add(kvp.Key);
                }
                if (kvp.Value == 0.25)
                {
                    ResistancesX4.Add(kvp.Key);
                }
                if (kvp.Value == 0)
                {
                    Immunites.Add(kvp.Key);
                }
            }
            foreach (KeyValuePair<TypeP, double> kvp in faiblesses)
            {
                if (kvp.Value == 2)
                {
                    FaiblessesX2.Add(kvp.Key);
                }
                if (kvp.Value == 4)
                {
                    FaiblessesX4.Add(kvp.Key);
                }
            }
        }


    }
}
