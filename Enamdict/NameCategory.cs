namespace Enamdict;

[Flags]
public enum NameCategory
{
    /*
     * s - surname (138,500)
     * p - place-name (99,500)
     * u - person name, either given or surname, as-yet unclassified (139,000) 
     * g - given name, as-yet not classified by sex (64,600)
     * f - female given name (106,300)
     * m - male given name (14,500)
     * h - full (usually family plus given) name of a particular person (30,500)
     * pr - product name (55)
     * c - company name (34)
     * o - organization name
     * st - stations (8,254)
     * wk - work of literature, art, film, etc.
     *
     * undocumented categories:
     * group - music band
     * obj - object
     * serv - service
     * 
     * ch - children
     * leg - legend
     * cr - creature
     * fic - fiction
     * ev - event
     * myth - mythology
     * dei - deity
     * ship
     * document
     */

    Unknown            = 0b_00000000_00000000_00000000_00000000,
    
    PersonUnclassified = 0b_00000000_00000000_00000000_00000001,
    PersonFull         = 0b_00000000_00000000_00000000_00000010,
    Surname            = 0b_00000000_00000000_00000000_00000100,
    GivenUnclassified  = 0b_00000000_00000000_00000000_00001000,
    GivenFemale        = 0b_00000000_00000000_00000000_00010000,
    GivenMale          = 0b_00000000_00000000_00000000_00100000,

    Place              = 0b_00000000_00000000_00000000_01000000,
    Product            = 0b_00000000_00000000_00000000_10000000,
    Company            = 0b_00000000_00000000_00000001_00000000,
    Organization       = 0b_00000000_00000000_00000010_00000000,
    Station            = 0b_00000000_00000000_00000100_00000000,
    Work               = 0b_00000000_00000000_00001000_00000000,
    
    Group              = 0b_00000000_00000000_00010000_00000000,
    Object             = 0b_00000000_00000000_00100000_00000000,
    Service            = 0b_00000000_00000000_01000000_00000000,
    Ship               = 0b_00000000_00000000_10000000_00000000,
}