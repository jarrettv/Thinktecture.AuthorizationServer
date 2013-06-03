﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> All { get; }
        void Add(T item);
        void Remove(T item);
    }

    public interface IAuthorizationServerAdministration
    {
        GlobalConfiguration GlobalConfiguration { get; }
        IRepository<Application> Applications { get; }
        IRepository<Client> Clients { get; }
        IRepository<SigningKey> Keys { get; }

        void SaveChanges();
    }
}