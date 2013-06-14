﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Applications)]
    public class ApplicationsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ApplicationsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var query =
                from a in config.Applications.All
                select new { a.ID, a.Namespace, a.Name, a.LogoUrl, a.Enabled };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }
        
        public HttpResponseMessage Get(int id)
        {
            var app = config.Applications.All.SingleOrDefault(x=>x.ID==id);
            if (app == null) 
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var data = new { 
                app.ID, 
                app.Name, 
                app.Description,
                app.LogoUrl,
                app.Namespace, 
                app.Audience,
                app.TokenLifetime,
                app.AllowRefreshToken,
                app.RequireConsent,
                app.RememberConsentDecision,
                signingKeyId = app.SigningKey.ID,
                enabled = app.Enabled
            };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        public HttpResponseMessage Put(int id, ApplicationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var app = config.Applications.All.SingleOrDefault(x => x.ID == id);
            if (app == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            app.Name = model.Name;
            app.Description = model.Description;
            app.LogoUrl = model.LogoUrl;
            app.Namespace = model.Namespace;
            app.Audience = model.Audience;
            app.TokenLifetime = model.TokenLifetime;
            app.AllowRefreshToken = model.AllowRefreshToken;
            app.RequireConsent = model.RequireConsent;
            app.RememberConsentDecision = model.RememberConsentDecision;
            app.SigningKey = config.Keys.All.Single(x => x.ID == model.SigningKeyID);
            app.Enabled = model.Enabled; ;

            config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Post(ApplicationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var app = new Application();
            app.Name = model.Name;
            app.Description = model.Description;
            app.LogoUrl = model.LogoUrl;
            app.Namespace = model.Namespace;
            app.Audience = model.Audience;
            app.TokenLifetime = model.TokenLifetime;
            app.AllowRefreshToken = model.AllowRefreshToken;
            app.RequireConsent = model.RequireConsent;
            app.RememberConsentDecision = model.RememberConsentDecision;
            app.SigningKey = config.Keys.All.Single(x => x.ID == model.SigningKeyID);
            app.Enabled = model.Enabled;

            config.Applications.Add(app);
            config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, new { ID = app.ID });
        }

        public HttpResponseMessage Delete(int id)
        {
            var app = this.config.Applications.All.SingleOrDefault(x => x.ID == id);
            if (app != null)
            {
                this.config.Applications.Remove(app);
                this.config.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

       
    }
}
