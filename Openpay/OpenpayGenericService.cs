﻿using System.Collections.Generic;

namespace Openpay
{
    public abstract class OpenpayGenericService
    {
        protected OpenpayHttpClient httpClient;

        public OpenpayGenericService(string api_key, string merchant_id, bool production = false)
        {
            this.httpClient = new OpenpayHttpClient(api_key, merchant_id, production);
        }

		internal OpenpayGenericService(OpenpayHttpClient opHttpClient)
        {
            this.httpClient = opHttpClient;
        }
			
        protected virtual T Get<T>(string ep)
        {
            return this.httpClient.Get<T>(ep);
        }

        protected List<T> List<T>(string url)
        {
            return this.httpClient.Get<List<T>>(url);
        }
    }
}
