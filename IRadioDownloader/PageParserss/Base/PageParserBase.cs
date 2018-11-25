using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.PageParsers.Base
{
    public abstract class PageDecoderBase
    {
        public IPageDecoder NextDecoder { get;  }

        public PageDecoderBase(IPageDecoder next)
        {
            NextDecoder = next;
        }
    }
}
