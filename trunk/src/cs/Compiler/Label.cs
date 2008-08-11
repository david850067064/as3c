/*
Copyright(C) 2007 Joa Ebert

As3c is an ActionScript 3 bytecode compiler for the AVM2.

As3c  is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

As3c is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace As3c.Compiler
{
    public class Label
    {
        private string _identifier;
        private bool _referenced;
        private uint _address;
        private bool _hasAddress;

        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        public bool Referenced
        {
            get { return _referenced; }
            set { _referenced = value; }
        }

        public uint Address
        {
            get
            {
                if (!_hasAddress)
                {
                    throw new Exception("Adress has not been set yet.");
                }

                return _address;
            }
            set
            {
                _address = value;
                _hasAddress = true;
            }
        }

        public bool HasAddress { get { return _hasAddress; } }

        public Label(string identifier)
        {
            _referenced = _hasAddress = false;
            _identifier = identifier;
        }
    }
}
