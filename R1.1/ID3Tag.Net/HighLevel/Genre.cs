using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Contains a dictionary of Genre IDs.
    /// </summary>
    public class Genre
    {
        private static Genre _instance;

        private readonly Dictionary<int, string> m_GenreDict;

        /// <summary>
        /// Creates a new instance of Genre
        /// </summary>
        private Genre()
        {
            m_GenreDict = GetDictionary();
        }

        /// <summary>
        /// Gets an instance of Genre.
        /// </summary>
        public static Genre Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Genre();
                }

                return _instance;
            }
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
                throw new Id3TagException("Genre id not found.");
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

    	/// <summary>
    	/// Gets the genres. The index of the list represents the id.
    	/// </summary>
    	/// <returns>A new readonly collection</returns>
    	public ReadOnlyCollection<string> AllGenres
    	{
    		get
    		{
    			var values = m_GenreDict.Values;
    			var list = new List<string>(values);

    			return new ReadOnlyCollection<string>(list);
    		}
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
                                 {80, "Folk"},
                                 {81, "Fock-Rock"},
                                 {82, "National Folk"},
                                 {83, "Swing"},
                                 {84, "Fast Fusion"},
                                 {85, "Bebop"},
                                 {86, "Latin"},
                                 {87, "Revival"},
                                 {88, "Celtic"},
                                 {89, "Bluegrass"},
                                 {90, "Avantgarde"},
                                 {91, "Gothic Rock"},
                                 {92, "Progressive Rock"},
                                 {93, "Psychedelic Rock"},
                                 {94, "Symphonic Rock"},
                                 {95, "Slow Rock"},
                                 {96, "Big Band"},
                                 {97, "Chorus"},
                                 {98, "Easy Listening"},
                                 {99, "Acoustic"},
                                 {100, "Humour"},
                                 {101, "Speech"},
                                 {102, "Chanson"},
                                 {103, "Opera"},
                                 {104, "Chamber Music"},
                                 {105, "Sonata"},
                                 {106, "Symphony"},
                                 {107, "Booty Bass"},
                                 {108, "Primus"},
                                 {109, "Porn Groove"},
                                 {110, "Satire"},
                                 {111, "Slow Jam"},
                                 {112, "Club"},
                                 {113, "Tango"},
                                 {114, "Samba"},
                                 {115, "Folklore"},
                                 {116, "Ballad"},
                                 {117, "Power Ballad"},
                                 {118, "Rhythmic Soul"},
                                 {119, "Free Style"},
                                 {120, "Duet"},
                                 {121, "Punk Rock"},
                                 {122, "Drum Solo"},
                                 {123, "A capella"},
                                 {124, "Euro-House"},
                                 {125, "Dance Hall"},
                             };

            return genres;
        }
    }
}