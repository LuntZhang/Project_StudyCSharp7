using System;
using System.Collections.Generic;
using System.Text;

namespace ListSamples
{
    class FindCountry
    {
        public FindCountry(string country) => _country = country;

        private readonly string _country;

        public bool FindCountryPredicate(Racer racer) => racer?.Country == _country;
    }
}
