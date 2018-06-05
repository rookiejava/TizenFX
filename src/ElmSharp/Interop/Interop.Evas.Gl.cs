/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Evas
    {
        [DllImport(Libraries.Evas)]
        internal static extern IntPtr evas_gl_new(IntPtr evas);

        [DllImport(Libraries.Evas)]
        internal static extern void evas_gl_free(IntPtr evas_gl);

        [DllImport(Libraries.Evas)]
        internal static extern IntPtr evas_gl_context_create(IntPtr evas_gl, IntPtr share_ctx);

        [DllImport(Libraries.Evas)]
        internal static extern void evas_gl_context_destroy(IntPtr evas_gl, IntPtr ctx);

        [DllImport(Libraries.Evas)]
        internal static extern IntPtr evas_gl_surface_create(IntPtr evas_gl, IntPtr config, int width, int height);

        [DllImport(Libraries.Evas)]
        internal static extern void evas_gl_surface_destroy(IntPtr evas_gl, IntPtr surf);

        [DllImport(Libraries.Evas)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool evas_gl_native_surface_get(IntPtr evas_gl, IntPtr surf, out IntPtr ns);

        [DllImport(Libraries.Evas)]
        internal static extern IntPtr evas_gl_proc_address_get(IntPtr evas_gl, string name);

        [DllImport(Libraries.Evas)]
        internal static extern IntPtr evas_gl_api_get(IntPtr evas_gl);

        [DllImport(Libraries.Evas)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool evas_gl_make_current(IntPtr evas_gl, IntPtr surf, IntPtr ctx);
    }
}