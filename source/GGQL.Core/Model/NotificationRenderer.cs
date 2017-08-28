using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace GGQL.Core
{

    public abstract class NotificationRenderer
    {
        private IConfiguration _Configuration = null;
        private IConfiguration _Null = null;
        public IConfiguration Configuration
        {
            set {
                this._Configuration = value;
            }
            get
            {
                if (_Configuration == null)
                {
                    if (_Null != null)
                    {
                        return _Null;
                    }
                    ConfigurationBuilder b = new ConfigurationBuilder();// ().AddInMemoryCollection();
                    _Null = b.Build();
                    return _Null;
                }
                return _Configuration;
            }
        }

        public abstract string BuildMessage(string[] eventGroups, Func<string, IEnumerable<IEvent>> groupGetter );
    }

}
