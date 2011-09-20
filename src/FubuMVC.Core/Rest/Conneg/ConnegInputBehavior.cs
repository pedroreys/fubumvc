using System;
using System.Collections.Generic;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Rest.Media;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    public class ConnegInputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IEnumerable<IMediaReader<T>> _readers;
        private readonly IOutputWriter _writer;
        private readonly IFubuRequest _request;

        public ConnegInputBehavior(IEnumerable<IMediaReader<T>> readers, IOutputWriter writer, IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _readers = readers;
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var mimeTypes = _request.Get<CurrentMimeType>();
            var reader = ChooseReader(mimeTypes);

            if (reader == null)
            {
                failWithInvalidMimeType();
                return DoNext.Stop;
            }

            var target = reader.Read(mimeTypes.ContentType.First());
            _request.Set(target);

            return DoNext.Continue;
        }

        private void failWithInvalidMimeType()
        {
            _writer.WriteResponseCode(HttpStatusCode.UnsupportedMediaType);
        }

        public IMediaReader<T> ChooseReader(CurrentMimeType mimeTypes)
        {
            return _readers.FirstOrDefault(x => x.Mimetypes.Contains(mimeTypes.ContentType.First()));
        }
    }
}