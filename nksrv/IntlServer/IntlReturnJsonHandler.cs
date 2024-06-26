﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using Swan;

namespace nksrv.IntlServer
{
    public class IntlReturnJsonHandler : IntlMsgHandler
    {
        private string JsonToReturn;
        public override bool RequiresAuth => false;

        public IntlReturnJsonHandler(string jsonToReturn)
        {
            JsonToReturn = jsonToReturn;
        }

        protected override async Task HandleAsync()
        {
            var str= await ctx.GetRequestBodyAsStringAsync();

            string? seg = ctx.GetRequestQueryData().Get("seq");
            WriteJsonString(JsonToReturn.Replace("((SEGID))", seg));
        }
    }
}
