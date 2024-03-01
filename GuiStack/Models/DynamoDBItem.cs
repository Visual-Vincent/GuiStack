/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace GuiStack.Models
{
    /// <summary>
    /// Represents an item or row in a DynamoDB table.
    /// </summary>
    public class DynamoDBItem : IDisposable
    {
        /// <summary>
        /// Gets or sets the collection of attributes that the item contains.
        /// </summary>
        public Dictionary<string, AttributeValue> Attributes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDBItem"/> class.
        /// </summary>
        public DynamoDBItem()
        {
            Attributes = new Dictionary<string, AttributeValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDBItem"/> class.
        /// </summary>
        /// <param name="attributes">The collection of attributes that this <see cref="DynamoDBItem"/> should contain.</param>
        public DynamoDBItem(IDictionary<string, AttributeValue> attributes)
        {
            if(attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            Attributes = new Dictionary<string, AttributeValue>(attributes);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    // Dispose managed objects

                    if(Attributes != null && Attributes.Count > 0)
                    {
                        var values = new Stack<AttributeValue>(Attributes.Values);
                        var disposed = new HashSet<AttributeValue>();

                        while(values.TryPop(out var value))
                        {
                            if(value == null || disposed.Contains(value))
                                continue;

                            value.B?.Dispose();
                            value.BS?.ForEach(ms => ms?.Dispose());

                            foreach(var innerValue in value.L ?? Enumerable.Empty<AttributeValue>())
                            {
                                values.Push(innerValue);
                            }

                            foreach(var innerValue in value.M?.Values ?? Enumerable.Empty<AttributeValue>())
                            {
                                values.Push(innerValue);
                            }

                            // To avoid infinite loop in case of circular references
                            disposed.Add(value);
                        }
                    }
                }

                // Free unmanaged resources
                // Set large fields to null

                disposedValue = true;
            }
        }

        // // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DynamoDBItem()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
