using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Contains a dictionary of Genre IDs.
    /// </summary>
    internal class Genre
    {
        private static Genre m_Instance;

        /// <summary>
        /// Gets an instance of Genre.
        /// </summary>
        public static Genre Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Genre();
                }

                return m_Instance;
            }
        }

        private readonly Dictionary<int, string> m_GenreDict;

        /// <summary>
        /// Creates a new instance of Genre
        /// </summary>
        private Genre()
        {
            m_GenreDict = GetDictionary();
        }

        /// <summary>
        /// Gets the description of the genre id.
        /// </summary>
        /// <param name="genreId">the id.</param>
        /// <returns>the description.</returns>
        public string GetGenre(int genreId)
        {
            var found = ContainsGenre(genreId);

            if (!found)
            {
                throw new ID3TagException("Genre id not found.");
            }

            var genreDescription = m_GenreDict[genreId];
            return genreDescription;
        }

        /// <summary>
        /// True if an description can be found, otherweise false.
        /// </summary>
        /// <param name="genreId">the gerne ID.</param>
        /// <returns>the description.</returns>
        public bool ContainsGenre(int genreId)
        {
            var found = m_GenreDict.ContainsKey(genreId);
            return found;
        }

        private static Dictionary<int, string> GetDictionary()
        {
            var genres = new Dictionary<int, string>
                             {
                                 {0, "Blues"},
                                 {1, "Classic Rock"},
                                 {2, "Country"},
                                 {3, "Dance"},
                                 {4, "Disco"},
                                 {5, "Funk"},
                                 {6, "Grunge"},
                                 {7, "Hip-Hop"},
                                 {8, "Jazz"},
                                 {9, "Metal"},
                                 {10, "New Age"},
                                 {11, "Oldies"},
                                 {12, "Other"},
                                 {13, "Pop"},
                                 {14, "R&B"},
                                 {15, "RAP"},
                                 {16, "Reggae"},
                                 {17, "Rock"},
                                 {18, "Techo"},
                                 {19, "Industrial"},
                                 {20, "Alternative"},
                                 {21, "Ska"},
                                 {22, "Death Metal"},
                                 {23, "Pranks"},
                                 {24, "Soundtrack"},
                                 {25, "Euro-Techno"},
                                 {26, "Ambient"},
                                 {27, "Trip-Hop"},
                                 {28, "Vocal"},
                                 {29, "Jazz&Funk"},
                                 {30, "Fusion"},
                                 {31, "Trance"},
                                 {32, "Classical"},
                                 {33, "Instrumental"},
                                 {34, "Acid"},
                                 {35, "House"},
                                 {36, "Game"},
                                 {37, "Sound Clip"},
                                 {38, "Gospel"},
                                 {39, "Noise"},
                                 {40, "Alternative Rock"},
                                 {41, "Bass"},
                                 {42, "Soul"},
                                 {43, "Punk"},
                                 {44, "Space"},
                                 {45, "Meditative"},
                                 {46, "Instrumental Pop"},
                                 {47, "Instrumental Rock"},
                                 {48, "Ethnic"},
                                 {49, "Gothic"},
                                 {50, "Darkwave"},
                                 {51, "Techo-Industrial"},
                                 {52, "Electronic"},
                                 {53, "Pop-Folk"},
                                 {54, "Eurodance"},
                                 {55, "Dream"},
                                 {56, "Southern Rock"},
                                 {57, "Comedy"},
                                 {58, "Cult"},
                                 {59, "Gangsta"},
                                 {60, "Top 40"},
                                 {61, "Christian Rap"},
                                 {62, "Pop/Funk"},
                                 {63, "Jungle"},
                                 {64, "Native US"},
                                 {65, "Cabaret"},
                                 {66, "New Wave"},
                                 {67, "Psychodelic"},
                                 {68, "Rave"},
                                 {69, "Showtunes"},
                                 {70, "Trailer"},
                                 {71, "Lo-Fi"},
                                 {72, "Tribal"},
                                 {73, "Acid Punk"},
                                 {74, "Acid Jazz"},
                                 {75, "Polka"},
                                 {76, "Retro"},
                                 {77, "Musical"},
                                 {78, "Rock & Roll"},
                                 {79, "Hard Rock"},
                                 {80, "Folk"}
                             };

            return genres;
        }
    }
}
