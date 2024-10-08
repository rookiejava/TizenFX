/*
 * Copyright(c) 2022 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.IO;
using System.Reflection;
using Tizen.NUI;
using System.Xml;
using Tizen.NUI.Binding.Internals;

// [assembly:Dependency(typeof(Tizen.NUI.Xaml.ResourcesLoader))]
namespace Tizen.NUI.Xaml
{
    internal class ResourcesLoader : IResourcesLoader
    {
        public T CreateFromResource<T>(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo) where T : new()
        {
            var rd = new T();
            var resourceLoadingResponse = ResourceLoader.ResourceProvider?.Invoke(new ResourceLoader.ResourceLoadingQuery
            {
                AssemblyName = assembly.GetName(),
                ResourcePath = resourcePath,
                Instance = rd
            });
            var alternateResource = resourceLoadingResponse?.ResourceContent;
            if (alternateResource != null)
            {
                rd.LoadFromXaml(alternateResource);
                return rd;
            }

            var resourceId = XamlResourceIdAttribute.GetResourceIdForPath(assembly, resourcePath);
            if (resourceId == null)
                throw new XamlParseException($"Resource '{resourcePath}' not found.", lineInfo);

            using (var stream = assembly.GetManifestResourceStream(resourceId))
            {
                if (stream == null)
                    throw new XamlParseException($"No resource found for '{resourceId}'.", lineInfo);
                using (var reader = new StreamReader(stream))
                {
                    rd.LoadFromXaml(reader.ReadToEnd());
                    return rd;
                }
            }
        }

        public string GetResource(string resourcePath, Assembly assembly, object target, IXmlLineInfo lineInfo)
        {
            var resourceLoadingResponse = ResourceLoader.ResourceProvider?.Invoke(new ResourceLoader.ResourceLoadingQuery {
                AssemblyName = assembly.GetName(),
                ResourcePath = resourcePath,
                Instance = target
            });
            var alternateResource = resourceLoadingResponse?.ResourceContent;
            if (alternateResource != null)
                return alternateResource;

            var resourceId = XamlResourceIdAttribute.GetResourceIdForPath(assembly, resourcePath);
            if (resourceId == null)
                throw new XamlParseException($"Resource '{resourcePath}' not found.", lineInfo);

            using (var stream = assembly.GetManifestResourceStream(resourceId))
            {
                if (stream == null)
                    throw new XamlParseException($"No resource found for '{resourceId}'.", lineInfo);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}
