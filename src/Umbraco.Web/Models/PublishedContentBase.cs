﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace Umbraco.Web.Models
{
    /// <summary>
    /// Provide an abstract base class for <c>IPublishedContent</c> implementations.
    /// </summary>
    /// <remarks>This base class does which (a) consistently resolves and caches the Url, (b) provides an implementation
    /// for this[alias], and (c) provides basic content set management.</remarks>
    [DebuggerDisplay("Content Id: {Id}}")]
    public abstract class PublishedContentBase : IPublishedContent
    {
        protected PublishedContentBase(IUmbracoContextAccessor umbracoContextAccessor)
        {
            UmbracoContextAccessor = umbracoContextAccessor ?? throw new ArgumentNullException(nameof(umbracoContextAccessor));
        }

        protected IUmbracoContextAccessor UmbracoContextAccessor { get; }

        #region ContentType

        public abstract IPublishedContentType ContentType { get; }

        #endregion

        #region PublishedElement

        /// <inheritdoc />
        public abstract Guid Key { get; }

        #endregion

        #region PublishedContent

        /// <inheritdoc />
        public abstract int Id { get; }

        /// <inheritdoc />
        public abstract string Name(string culture = null);

        /// <inheritdoc />
        public abstract string UrlSegment(string culture = null);

        /// <inheritdoc />
        public abstract int SortOrder { get; }

        /// <inheritdoc />
        public abstract int Level { get; }

        /// <inheritdoc />
        public abstract string Path { get; }

        /// <inheritdoc />
        public abstract int? TemplateId { get; }

        /// <inheritdoc />
        public abstract int CreatorId { get; }

        /// <inheritdoc />
        public abstract string CreatorName { get; }

        /// <inheritdoc />
        public abstract DateTime CreateDate { get; }

        /// <inheritdoc />
        public abstract int WriterId { get; }

        /// <inheritdoc />
        public abstract string WriterName { get; }

        /// <inheritdoc />
        public abstract DateTime UpdateDate { get; }

        /// <inheritdoc />
        /// <remarks>
        /// The url of documents are computed by the document url providers. The url of medias are computed by the media url providers.
        /// </remarks>
        public virtual string Url(string culture = null, UrlMode mode = UrlMode.Auto)
        {
            var umbracoContext = UmbracoContextAccessor.UmbracoContext;

            if (umbracoContext == null)
                throw new InvalidOperationException("Cannot compute Url for a content item when UmbracoContext is null.");
            if (umbracoContext.UrlProvider == null)
                throw new InvalidOperationException("Cannot compute Url for a content item when UmbracoContext.UrlProvider is null.");

            switch (ContentType.ItemType)
            {
                case PublishedItemType.Content:
                    return umbracoContext.UrlProvider.GetUrl(this, mode, culture);

                case PublishedItemType.Media:
                    return umbracoContext.UrlProvider.GetMediaUrl(this, Constants.Conventions.Media.File, mode, culture);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public abstract DateTime CultureDate(string culture = null);

        /// <inheritdoc />
        public abstract IReadOnlyCollection<string> Cultures { get; }

        /// <inheritdoc />
        public abstract bool IsDraft(string culture = null);

        /// <inheritdoc />
        public abstract bool IsPublished(string culture = null);

        #endregion

        #region Tree

        /// <inheritdoc />
        public abstract IPublishedContent Parent();

        /// <inheritdoc />
        public abstract IEnumerable<IPublishedContent> Children(string culture = null);

        #endregion

        #region Properties

        /// <inheritdoc cref="IPublishedElement.Properties"/>
        public abstract IEnumerable<IPublishedProperty> Properties { get; }

        /// <inheritdoc cref="IPublishedElement.GetProperty(string)"/>
        public abstract IPublishedProperty GetProperty(string alias);

        #endregion
    }
}
