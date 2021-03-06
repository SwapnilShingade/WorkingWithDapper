﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DataLayers
{
    public class Contact
    {
        public int Id { get; set; }
        public string  FirstName { get; set; }
        public string  LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        [Computed]
        public bool IsNew => this.Id == default;
        [Write(false)]
        public List<Address> Addresses { get; set; }

    }
}
