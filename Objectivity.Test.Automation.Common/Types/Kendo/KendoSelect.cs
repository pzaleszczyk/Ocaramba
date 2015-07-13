﻿/*
The MIT License (MIT)

Copyright (c) 2015 Objectivity Bespoke Software Specialists

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Objectivity.Test.Automation.Common.Types.Kendo
{
    using System.Collections.ObjectModel;
    using System.Globalization;

    using Objectivity.Test.Automation.Common.Extensions;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;

    /// <summary>
    /// Kendo Select element
    /// </summary>
    public abstract class KendoSelect : RemoteWebElement
    {
        private readonly IWebElement webElement;

        private readonly string elementCssSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="KendoSelect"/> class.
        /// </summary>
        /// <param name="webElement">The webElement</param>
        protected KendoSelect(IWebElement webElement)
            : base(webElement.ToDriver() as RemoteWebDriver, null)
        {
            this.webElement = webElement;
            var id = this.webElement.GetHiddenElement(new ElementLocator(Locator.XPath, "./ancestor-or-self::span[contains(@class, 'k-widget')]//input[@id]")).GetAttribute("id");
            this.elementCssSelector = string.Format(CultureInfo.InvariantCulture, "#{0}", id);
        }

        /// <summary>
        /// Gets the driver.
        /// </summary>
        public IWebDriver Browser
        {
            get
            {
                return this.webElement.ToDriver();
            }
        }

        /// <summary>Gets the unordered list.</summary>
        /// <value>The unordered list.</value>
        public IWebElement UnorderedList
        {
            get
            {
                object elements = this.Browser.Scripts()
                .ExecuteScript(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "return $('{0}').data('{1}').ul.toArray();",
                        this.elementCssSelector,
                        this.SelectType));

                var webElements = elements as ReadOnlyCollection<IWebElement>;
                return webElements != null ? webElements[0] : null;
            }
        }

        /// <summary>Gets the selector.</summary>
        /// <value>The selector.</value>
        protected abstract string SelectType { get; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public Collection<string> Options
        {
            get
            {
                var count =
                (long)
                this.Browser.Scripts()
                    .ExecuteScript(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "return $('{0}').data('{1}').dataSource.data().length;",
                            this.elementCssSelector,
                            this.SelectType));
                var options = new Collection<string>();
                for (var i = 0; i < count; i++)
                {
                    options.Add(
                        (string)
                        this.Browser.Scripts()
                            .ExecuteScript(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "return $('{0}').data('{1}').ul.children().eq({2}).text();",
                                    this.elementCssSelector,
                                    this.SelectType,
                                    i)));
                }

                return options;
            }
        }

        /// <summary>Gets the selected option.</summary>
        /// <value>The selected option.</value>
        public string SelectedOption
        {
            get
            {
                var option =
                    (string)
                    this.Browser.Scripts()
                        .ExecuteScript(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "return $('{0}').data('{1}').text();",
                                this.elementCssSelector,
                                this.SelectType));
                return option;
            }
        }

        /// <summary>Select by text.</summary>
        /// <param name="text">The text.</param>
        public void SelectByText(string text)
        {
            this.Browser.Scripts().ExecuteScript(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "$('{0}').data('{1}').select(function(dataItem) {{return dataItem.text === '{2}';}});",
                    this.elementCssSelector,
                    this.SelectType,
                    text));
        }

        /// <summary>Closes this object.</summary>
        public void Close()
        {
            this.Browser.Scripts()
                .ExecuteScript(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "$('{0}').data('{1}').close();",
                        this.elementCssSelector,
                        this.SelectType));
        }

        /// <summary>Opens this object.</summary>
        public void Open()
        {
            this.Browser.Scripts()
                .ExecuteScript(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "$('{0}').data('{1}').open();",
                        this.elementCssSelector,
                        this.SelectType));
        }
    }
}
