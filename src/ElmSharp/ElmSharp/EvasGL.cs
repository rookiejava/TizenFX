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
using System.Reflection;
using System.Runtime.InteropServices;

namespace ElmSharp
{
    /// <summary>
    /// The EvasGL is for GL rendering with the EFL.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public class EvasGL
    {
        IntPtr _evasGL;
        IntPtr _evasGlConfig;

        /// <summary>
        /// Constructor for the EvasGL class.
        /// </summary>
        /// <param name="parent"></param>
        public EvasGL(EvasObject parent)
        {
            _evasGL = Interop.Evas.evas_gl_new(Interop.Evas.evas_object_evas_get(parent));
        }

        /// <summary>
        /// Destructor for the EvasGL class.
        /// </summary>
        ~EvasGL()
        {
            Interop.Evas.evas_gl_free(_evasGL);
            _evasGL = IntPtr.Zero;
        }

        internal IntPtr Handle => _evasGL;

        /// <summary>
        /// Create the EvasGLContext
        /// </summary>
        /// <returns>EvasGLContext</returns>
        /// <since_tizen> preview </since_tizen>
        public EvasGLContext CreateEvasGLContext()
        {
            return new EvasGLContext(Handle);
        }

        /// <summary>
        /// Destroy yhe EvasGLContext
        /// </summary>
        /// <param name="glContext">EvasGLContext to be destroyed.</param>
        /// <since_tizen> preview </since_tizen>
        public void DestroyEvasGLContext(EvasGLContext glContext)
        {
            Interop.Evas.evas_gl_context_destroy(Handle, glContext.Handle);
        }

        /// <summary>
        /// Create the EvasGLSurface
        /// </summary>
        /// <param name="config">EvasGLConfig</param>
        /// <param name="w">Width of surface</param>
        /// <param name="h">Height of surface</param>
        /// <returns>EvasGLSurface</returns>
        /// <since_tizen> preview </since_tizen>
        public EvasGLSurface CreateEvasGLSurface(EvasGLConfig config, int w, int h)
        {
            if (_evasGlConfig != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_evasGlConfig);
                _evasGlConfig = IntPtr.Zero;
            }
            _evasGlConfig = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            Marshal.StructureToPtr(config, _evasGlConfig, false);
            return new EvasGLSurface(Handle, _evasGlConfig, w, h);
        }

        /// <summary>
        /// Destroy the EvasGLSurface
        /// </summary>
        /// <param name="glSurface">EvasGLSurface to be destroyed.</param>
        /// <since_tizen> preview </since_tizen>
        public void DestroyEvasGLSurface(EvasGLSurface glSurface)
        {
            if (_evasGlConfig != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_evasGlConfig);
                _evasGlConfig = IntPtr.Zero;
            }
            Interop.Evas.evas_gl_surface_destroy(Handle, glSurface.Handle);
        }

        /// <summary>
        /// Sets the given context as the current context for the given surface.
        /// </summary>
        /// <param name="glSurface">The given Evas GL surface</param>
        /// <param name="glContext">The given Evas GL context</param>
        /// <since_tizen> preview </since_tizen>
        public void MakeCurrent(EvasGLSurface glSurface, EvasGLContext glContext)
        {
            Interop.Evas.evas_gl_make_current(Handle, glSurface.Handle, glContext.Handle);
        }

        /// <summary>
        /// Fills in the Native Surface information from a given Evas GL surface.
        /// </summary>
        /// <param name="glSurface">The given Evas GL surface to retrieve the Native Surface information from.</param>
        /// <returns>The native surface structure that the function fills in.</returns>
        /// <since_tizen> preview </since_tizen>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public IntPtr GetNativeGLSurface(EvasGLSurface glSurface)
        {
            IntPtr nativeSurface;
            Interop.Evas.evas_gl_native_surface_get(Handle, glSurface.Handle, out nativeSurface);
            return nativeSurface;
        }

        /// <summary>
        /// Returns a extension function from the Evas_GL glue layer.
        /// </summary>
        /// <param name="name">The name of the function to return</param>
        /// <returns>A function pointer to the Evas_GL extension.</returns>
        /// <since_tizen> preview </since_tizen>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public IntPtr GetFunctionPointer(string name)
        {
            var ret = Interop.Evas.evas_gl_proc_address_get(Handle, name);

            if (ret == IntPtr.Zero)
            {
                var field = typeof(EvasGLAPI).GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field?.FieldType == typeof(IntPtr))
                    ret = (IntPtr)field.GetValue(GetEvasGLAPI());
            }
            return ret;
        }

        /// <summary>
        /// Gets the API for rendering using OpenGL.
        /// </summary>
        /// <returns>EvasGLAPI</returns>
        /// <since_tizen> preview </since_tizen>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        internal EvasGLAPI GetEvasGLAPI()
        {
            var unmanagedGlApi = Interop.Evas.evas_gl_api_get(Handle);
            return  Marshal.PtrToStructure<EvasGLAPI>(unmanagedGlApi);
        }
    }

    /// <summary>
    /// The EvasGL is for GL rendering with the EFL.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public class EvasGLContext
    {
        IntPtr _evasGLContext;

        internal EvasGLContext(IntPtr glEvas)
        {
            _evasGLContext = Interop.Evas.evas_gl_context_create(glEvas, IntPtr.Zero);
        }

        internal EvasGLContext(IntPtr glEvas, IntPtr glContext)
        {
            _evasGLContext = Interop.Evas.evas_gl_context_create(glEvas, glContext);
        }

        internal IntPtr Handle => _evasGLContext;
    }

    /// <summary>
    /// Evas GL Surface object, a GL rendering target in Evas GL.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public class EvasGLSurface
    {
        IntPtr _glSurface;

        internal EvasGLSurface(IntPtr glEvas, IntPtr glConfig, int w, int h)
        {
            _glSurface = Interop.Evas.evas_gl_surface_create(glEvas, glConfig, w, h);
        }

        internal IntPtr Handle => _glSurface;
    }


    /// <summary>
    /// Evas GL Surface configuration object for surface creation.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public struct EvasGLConfig
    {
        /// <summary>
        /// Surface color format
        /// </summary>
        /// <since_tizen> preview </since_tizen>
        public ColorFormat colorFormat;

        /// <summary>
        /// Surface depth bits
        /// </summary>
        /// <since_tizen> preview </since_tizen>
        public DepthBits depthBits;

        /// <summary>
        /// Surface stencil bits
        /// </summary>
        /// <since_tizen> preview </since_tizen>
        public StencilBits stencilBits;

        /// <summary>
        /// Surface options bits
        /// </summary>
        /// <since_tizen> preview </since_tizen>
        public OptionsBits optionsBits;

        /// <summary>
        /// Surface multi-sample bits
        /// </summary>
        /// <since_tizen> preview </since_tizen>
        public MultisampleBits multisampleBits;

        /// <summary>
        /// OpenGL ES version number
        /// </summary>
        /// <since_tizen> preview </since_tizen>
        public GLContextVersion glesVersion;
    }

    /// <summary>
    /// Enumeration that defines the available surface color formats.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public enum ColorFormat
    {
        /// <summary>
        /// Opaque RGB surface
        /// </summary>
        Rgb888,

        /// <summary>
        /// RGBA surface with alpha
        /// </summary>
        Rgba8888,

        /// <summary>
        /// Special value for creating PBuffer surfaces without any attached buffer.
        /// </summary>
        NoFBO
    }

    /// <summary>
    /// Enumeration that defines the Surface Depth Format.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public enum DepthBits
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// 8 bits precision surface depth
        /// </summary>
        BIT_8,

        /// <summary>
        /// 16 bits precision surface depth
        /// </summary>
        BIT_16,

        /// <summary>
        /// 24 bits precision surface depth
        /// </summary>
        BIT_24,

        /// <summary>
        /// 32 bits precision surface depth
        /// </summary>
        BIT_32
    }

    /// <summary>
    /// Enumeration that defines the Surface Stencil Format.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public enum StencilBits
    {
        /// <summary>
        /// None
        /// </summary>
        NONE,

        /// <summary>
        /// 1 bit precision for stencil buffer
        /// </summary>
        BIT_1,

        /// <summary>
        /// 2 bits precision for stencil buffer
        /// </summary>
        BIT_2,

        /// <summary>
        /// 4 bits precision for stencil buffer
        /// </summary>
        BIT_4,

        /// <summary>
        /// 8 bits precision for stencil buffer
        /// </summary>
        BIT_8,

        /// <summary>
        /// 16 bits precision for stencil buffer
        /// </summary>
        BIT_16
    }

    /// <summary>
    /// Enumeration that defines the Configuration Options.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public enum OptionsBits
    {
        /// <summary>
        /// No extra options
        /// </summary>
        None = 0,

        /// <summary>
        /// Optional hint to allow rendering directly to the Evas window if possible
        /// </summary>
        Direct = (1 << 0),

        /// <summary>
        /// Force direct rendering even if the canvas is rotated
        /// </summary>
        ClientSideRotation = (1 << 1),

        /// <summary>
        /// If enabled, Evas GL pixel callback will be called by another thread instead of main thread
        /// </summary>
        Thread = (1 << 2)
    }

    /// <summary>
    /// Enumeration that defines the configuration options for a Multisample Anti-Aliased (MSAA) rendering surface.
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public enum MultisampleBits
    {
        /// <summary>
        /// No multisample rendering
        /// </summary>
        None,

        /// <summary>
        /// MSAA with minimum number of samples
        /// </summary>
        Low,

        /// <summary>
        /// MSAA with half the maximum number of samples
        /// </summary>
        Medium,

        /// <summary>
        /// MSAA with maximum allowed samples
        /// </summary>
        High
    }

    /// <summary>
    /// Enumeration that defines the available OpenGL ES version numbers. They can be used to create OpenGL-ES 1.1 contexts. 
    /// </summary>
    /// <since_tizen> preview </since_tizen>
    public enum GLContextVersion
    {
        /// <summary>
        /// OpenGL-ES 1.x
        /// </summary>
        GLES_1_X = 1,

        /// <summary>
        /// OpenGL-ES 2.x is the default
        /// </summary>
        GLES_2_X = 2,

        /// <summary>
        /// OpenGL-ES 3.
        /// </summary>
        GLES_3_X = 3,

        /// <summary>
        /// Enable debug mode on this context
        /// </summary>
        DEBUG = 0x1000
    }

    // this structure is initialized from a native pointer
    internal struct EvasGLAPI
    {
        // DO NOT change the order, needs to be as specified in struct _Evas_GL_API (/platform/upstream/efl/src/lib/evas/Evas_GL.h)
        // DO NOT change the names, they need to match the OpenGL API
#pragma warning disable 0169
        int version;
        IntPtr glActiveTexture;
        IntPtr glAttachShader;
        IntPtr glBindAttribLocation;
        IntPtr glBindBuffer;
        IntPtr glBindFramebuffer;
        IntPtr glBindRenderbuffer;
        IntPtr glBindTexture;
        IntPtr glBlendColor;
        IntPtr glBlendEquation;
        IntPtr glBlendEquationSeparate;
        IntPtr glBlendFunc;
        IntPtr glBlendFuncSeparate;
        IntPtr glBufferData;
        IntPtr glBufferSubData;
        IntPtr glCheckFramebufferStatus;
        IntPtr glClear;
        IntPtr glClearColor;
        IntPtr glClearDepthf;
        IntPtr glClearStencil;
        IntPtr glColorMask;
        IntPtr glCompileShader;
        IntPtr glCompressedTexImage2D;
        IntPtr glCompressedTexSubImage2D;
        IntPtr glCopyTexImage2D;
        IntPtr glCopyTexSubImage2D;
        IntPtr glCreateProgram;
        IntPtr glCreateShader;
        IntPtr glCullFace;
        IntPtr glDeleteBuffers;
        IntPtr glDeleteFramebuffers;
        IntPtr glDeleteProgram;
        IntPtr glDeleteRenderbuffers;
        IntPtr glDeleteShader;
        IntPtr glDeleteTextures;
        IntPtr glDepthFunc;
        IntPtr glDepthMask;
        IntPtr glDepthRangef;
        IntPtr glDetachShader;
        IntPtr glDisable;
        IntPtr glDisableVertexAttribArray;
        IntPtr glDrawArrays;
        IntPtr glDrawElements;
        IntPtr glEnable;
        IntPtr glEnableVertexAttribArray;
        IntPtr glFinish;
        IntPtr glFlush;
        IntPtr glFramebufferRenderbuffer;
        IntPtr glFramebufferTexture2D;
        IntPtr glFrontFace;
        IntPtr glGenBuffers;
        IntPtr glGenerateMipmap;
        IntPtr glGenFramebuffers;
        IntPtr glGenRenderbuffers;
        IntPtr glGenTextures;
        IntPtr glGetActiveAttrib;
        IntPtr glGetActiveUniform;
        IntPtr glGetAttachedShaders;
        IntPtr glGetAttribLocation;
        IntPtr glGetBooleanv;
        IntPtr glGetBufferParameteriv;
        IntPtr glGetError;
        IntPtr glGetFloatv;
        IntPtr glGetFramebufferAttachmentParameteriv;
        IntPtr glGetIntegerv;
        IntPtr glGetProgramiv;
        IntPtr glGetProgramInfoLog;
        IntPtr glGetRenderbufferParameteriv;
        IntPtr glGetShaderiv;
        IntPtr glGetShaderInfoLog;
        IntPtr glGetShaderPrecisionFormat;
        IntPtr glGetShaderSource;
        IntPtr glGetString;
        IntPtr glGetTexParameterfv;
        IntPtr glGetTexParameteriv;
        IntPtr glGetUniformfv;
        IntPtr glGetUniformiv;
        IntPtr glGetUniformLocation;
        IntPtr glGetVertexAttribfv;
        IntPtr glGetVertexAttribiv;
        IntPtr glGetVertexAttribPointerv;
        IntPtr glHint;
        IntPtr glIsBuffer;
        IntPtr glIsEnabled;
        IntPtr glIsFramebuffer;
        IntPtr glIsProgram;
        IntPtr glIsRenderbuffer;
        IntPtr glIsShader;
        IntPtr glIsTexture;
        IntPtr glLineWidth;
        IntPtr glLinkProgram;
        IntPtr glPixelStorei;
        IntPtr glPolygonOffset;
        IntPtr glReadPixels;
        IntPtr glReleaseShaderCompiler;
        IntPtr glRenderbufferStorage;
        IntPtr glSampleCoverage;
        IntPtr glScissor;
        IntPtr glShaderBinary;
        IntPtr glShaderSource;
        IntPtr glStencilFunc;
        IntPtr glStencilFuncSeparate;
        IntPtr glStencilMask;
        IntPtr glStencilMaskSeparate;
        IntPtr glStencilOp;
        IntPtr glStencilOpSeparate;
        IntPtr glTexImage2D;
        IntPtr glTexParameterf;
        IntPtr glTexParameterfv;
        IntPtr glTexParameteri;
        IntPtr glTexParameteriv;
        IntPtr glTexSubImage2D;
        IntPtr glUniform1f;
        IntPtr glUniform1fv;
        IntPtr glUniform1i;
        IntPtr glUniform1iv;
        IntPtr glUniform2f;
        IntPtr glUniform2fv;
        IntPtr glUniform2i;
        IntPtr glUniform2iv;
        IntPtr glUniform3f;
        IntPtr glUniform3fv;
        IntPtr glUniform3i;
        IntPtr glUniform3iv;
        IntPtr glUniform4f;
        IntPtr glUniform4fv;
        IntPtr glUniform4i;
        IntPtr glUniform4iv;
        IntPtr glUniformMatrix2fv;
        IntPtr glUniformMatrix3fv;
        IntPtr glUniformMatrix4fv;
        IntPtr glUseProgram;
        IntPtr glValidateProgram;
        IntPtr glVertexAttrib1f;
        IntPtr glVertexAttrib1fv;
        IntPtr glVertexAttrib2f;
        IntPtr glVertexAttrib2fv;
        IntPtr glVertexAttrib3f;
        IntPtr glVertexAttrib3fv;
        IntPtr glVertexAttrib4f;
        IntPtr glVertexAttrib4fv;
        IntPtr glVertexAttribPointer;
        IntPtr glViewport;
        IntPtr glEvasGLImageTargetTexture2DOES;
        IntPtr glEvasGLImageTargetRenderbufferStorageOES;
        IntPtr glGetProgramBinaryOES;
        IntPtr glProgramBinaryOES;
        IntPtr glMapBufferOES;
        IntPtr glUnmapBufferOES;
        IntPtr glGetBufferPointervOES;
        IntPtr glTexImage3DOES;
        IntPtr glTexSubImage3DOES;
        IntPtr glCopyTexSubImage3DOES;
        IntPtr glCompressedTexImage3DOES;
        IntPtr glCompressedTexSubImage3DOES;
        IntPtr glFramebufferTexture3DOES;
        IntPtr glGetPerfMonitorGroupsAMD;
        IntPtr glGetPerfMonitorCountersAMD;
        IntPtr glGetPerfMonitorGroupStringAMD;
        IntPtr glGetPerfMonitorCounterStringAMD;
        IntPtr glGetPerfMonitorCounterInfoAMD;
        IntPtr glGenPerfMonitorsAMD;
        IntPtr glDeletePerfMonitorsAMD;
        IntPtr glSelectPerfMonitorCountersAMD;
        IntPtr glBeginPerfMonitorAMD;
        IntPtr glEndPerfMonitorAMD;
        IntPtr glGetPerfMonitorCounterDataAMD;
        IntPtr glDiscardFramebufferEXT;
        IntPtr glMultiDrawArraysEXT;
        IntPtr glMultiDrawElementsEXT;
        IntPtr glDeleteFencesNV;
        IntPtr glGenFencesNV;
        IntPtr glIsFenceNV;
        IntPtr glTestFenceNV;
        IntPtr glGetFenceivNV;
        IntPtr glFinishFenceNV;
        IntPtr glSetFenceNV;
        IntPtr glGetDriverControlsQCOM;
        IntPtr glGetDriverControlStringQCOM;
        IntPtr glEnableDriverControlQCOM;
        IntPtr glDisableDriverControlQCOM;
        IntPtr glExtGetTexturesQCOM;
        IntPtr glExtGetBuffersQCOM;
        IntPtr glExtGetRenderbuffersQCOM;
        IntPtr glExtGetFramebuffersQCOM;
        IntPtr glExtGetTexLevelParameterivQCOM;
        IntPtr glExtTexObjectStateOverrideiQCOM;
        IntPtr glExtGetTexSubImageQCOM;
        IntPtr glExtGetBufferPointervQCOM;
        IntPtr glExtGetShadersQCOM;
        IntPtr glExtGetProgramsQCOM;
        IntPtr glExtIsProgramBinaryQCOM;
        IntPtr glExtGetProgramBinarySourceQCOM;
        IntPtr evasglCreateImage;
        IntPtr evasglDestroyImage;
        IntPtr evasglCreateImageForContext;
        IntPtr glAlphaFunc;
        IntPtr glClipPlanef;
        IntPtr glColor4f;
        IntPtr glFogf;
        IntPtr glFogfv;
        IntPtr glFrustumf;
        IntPtr glGetClipPlanef;
        IntPtr glGetLightfv;
        IntPtr glGetMaterialfv;
        IntPtr glGetTexEnvfv;
        IntPtr glLightModelf;
        IntPtr glLightModelfv;
        IntPtr glLightf;
        IntPtr glLightfv;
        IntPtr glLoadMatrixf;
        IntPtr glMaterialf;
        IntPtr glMaterialfv;
        IntPtr glMultMatrixf;
        IntPtr glMultiTexCoord4f;
        IntPtr glNormal3f;
        IntPtr glOrthof;
        IntPtr glPointParameterf;
        IntPtr glPointParameterfv;
        IntPtr glPointSize;
        IntPtr glPointSizePointerOES;
        IntPtr glRotatef;
        IntPtr glScalef;
        IntPtr glTexEnvf;
        IntPtr glTexEnvfv;
        IntPtr glTranslatef;
        IntPtr glAlphaFuncx;
        IntPtr glClearColorx;
        IntPtr glClearDepthx;
        IntPtr glClientActiveTexture;
        IntPtr glClipPlanex;
        IntPtr glColor4ub;
        IntPtr glColor4x;
        IntPtr glColorPointer;
        IntPtr glDepthRangex;
        IntPtr glDisableClientState;
        IntPtr glEnableClientState;
        IntPtr glFogx;
        IntPtr glFogxv;
        IntPtr glFrustumx;
        IntPtr glGetClipPlanex;
        IntPtr glGetFixedv;
        IntPtr glGetLightxv;
        IntPtr glGetMaterialxv;
        IntPtr glGetPointerv;
        IntPtr glGetTexEnviv;
        IntPtr glGetTexEnvxv;
        IntPtr glGetTexParameterxv;
        IntPtr glLightModelx;
        IntPtr glLightModelxv;
        IntPtr glLightx;
        IntPtr glLightxv;
        IntPtr glLineWidthx;
        IntPtr glLoadIdentity;
        IntPtr glLoadMatrixx;
        IntPtr glLogicOp;
        IntPtr glMaterialx;
        IntPtr glMaterialxv;
        IntPtr glMatrixMode;
        IntPtr glMultMatrixx;
        IntPtr glMultiTexCoord4x;
        IntPtr glNormal3x;
        IntPtr glNormalPointer;
        IntPtr glOrthox;
        IntPtr glPointParameterx;
        IntPtr glPointParameterxv;
        IntPtr glPointSizex;
        IntPtr glPolygonOffsetx;
        IntPtr glPopMatrix;
        IntPtr glPushMatrix;
        IntPtr glRotatex;
        IntPtr glSampleCoveragex;
        IntPtr glScalex;
        IntPtr glShadeModel;
        IntPtr glTexCoordPointer;
        IntPtr glTexEnvi;
        IntPtr glTexEnvx;
        IntPtr glTexEnviv;
        IntPtr glTexEnvxv;
        IntPtr glTexParameterx;
        IntPtr glTexParameterxv;
        IntPtr glTranslatex;
        IntPtr glVertexPointer;
        IntPtr glBlendEquationSeparateOES;
        IntPtr glBlendFuncSeparateOES;
        IntPtr glBlendEquationOES;
        IntPtr glDrawTexsOES;
        IntPtr glDrawTexiOES;
        IntPtr glDrawTexxOES;
        IntPtr glDrawTexsvOES;
        IntPtr glDrawTexivOES;
        IntPtr glDrawTexxvOES;
        IntPtr glDrawTexfOES;
        IntPtr glDrawTexfvOES;
        IntPtr glAlphaFuncxOES;
        IntPtr glClearColorxOES;
        IntPtr glClearDepthxOES;
        IntPtr glClipPlanexOES;
        IntPtr glColor4xOES;
        IntPtr glDepthRangexOES;
        IntPtr glFogxOES;
        IntPtr glFogxvOES;
        IntPtr glFrustumxOES;
        IntPtr glGetClipPlanexOES;
        IntPtr glGetFixedvOES;
        IntPtr glGetLightxvOES;
        IntPtr glGetMaterialxvOES;
        IntPtr glGetTexEnvxvOES;
        IntPtr glGetTexParameterxvOES;
        IntPtr glLightModelxOES;
        IntPtr glLightModelxvOES;
        IntPtr glLightxOES;
        IntPtr glLightxvOES;
        IntPtr glLineWidthxOES;
        IntPtr glLoadMatrixxOES;
        IntPtr glMaterialxOES;
        IntPtr glMaterialxvOES;
        IntPtr glMultMatrixxOES;
        IntPtr glMultiTexCoord4xOES;
        IntPtr glNormal3xOES;
        IntPtr glOrthoxOES;
        IntPtr glPointParameterxOES;
        IntPtr glPointParameterxvOES;
        IntPtr glPointSizexOES;
        IntPtr glPolygonOffsetxOES;
        IntPtr glRotatexOES;
        IntPtr glSampleCoveragexOES;
        IntPtr glScalexOES;
        IntPtr glTexEnvxOES;
        IntPtr glTexEnvxvOES;
        IntPtr glTexParameterxOES;
        IntPtr glTexParameterxvOES;
        IntPtr glTranslatexOES;
        IntPtr glIsRenderbufferOES;
        IntPtr glBindRenderbufferOES;
        IntPtr glDeleteRenderbuffersOES;
        IntPtr glGenRenderbuffersOES;
        IntPtr glRenderbufferStorageOES;
        IntPtr glGetRenderbufferParameterivOES;
        IntPtr glIsFramebufferOES;
        IntPtr glBindFramebufferOES;
        IntPtr glDeleteFramebuffersOES;
        IntPtr glGenFramebuffersOES;
        IntPtr glCheckFramebufferStatusOES;
        IntPtr glFramebufferRenderbufferOES;
        IntPtr glFramebufferTexture2DOES;
        IntPtr glGetFramebufferAttachmentParameterivOES;
        IntPtr glGenerateMipmapOES;
        IntPtr glCurrentPaletteMatrixOES;
        IntPtr glLoadPaletteFromModelViewMatrixOES;
        IntPtr glMatrixIndexPointerOES;
        IntPtr glWeightPointerOES;
        IntPtr glQueryMatrixxOES;
        IntPtr glDepthRangefOES;
        IntPtr glFrustumfOES;
        IntPtr glOrthofOES;
        IntPtr glClipPlanefOES;
        IntPtr glGetClipPlanefOES;
        IntPtr glClearDepthfOES;
        IntPtr glTexGenfOES;
        IntPtr glTexGenfvOES;
        IntPtr glTexGeniOES;
        IntPtr glTexGenivOES;
        IntPtr glTexGenxOES;
        IntPtr glTexGenxvOES;
        IntPtr glGetTexGenfvOES;
        IntPtr glGetTexGenivOES;
        IntPtr glGetTexGenxvOES;
        IntPtr glBindVertexArrayOES;
        IntPtr glDeleteVertexArraysOES;
        IntPtr glGenVertexArraysOES;
        IntPtr glIsVertexArrayOES;
        IntPtr glCopyTextureLevelsAPPLE;
        IntPtr glRenderbufferStorageMultisampleAPPLE;
        IntPtr glResolveMultisampleFramebufferAPPLE;
        IntPtr glFenceSyncAPPLE;
        IntPtr glIsSyncAPPLE;
        IntPtr glDeleteSyncAPPLE;
        IntPtr glClientWaitSyncAPPLE;
        IntPtr glWaitSyncAPPLE;
        IntPtr glGetInteger64vAPPLE;
        IntPtr glGetSyncivAPPLE;
        IntPtr glMapBufferRangeEXT;
        IntPtr glFlushMappedBufferRangeEXT;
        IntPtr glRenderbufferStorageMultisampleEXT;
        IntPtr glFramebufferTexture2DMultisampleEXT;
        IntPtr glGetGraphicsResetStatusEXT;
        IntPtr glReadnPixelsEXT;
        IntPtr glGetnUniformfvEXT;
        IntPtr glGetnUniformivEXT;
        IntPtr glTexStorage1DEXT;
        IntPtr glTexStorage2DEXT;
        IntPtr glTexStorage3DEXT;
        IntPtr glTextureStorage1DEXT;
        IntPtr glTextureStorage2DEXT;
        IntPtr glTextureStorage3DEXT;
        IntPtr glClipPlanefIMG;
        IntPtr glClipPlanexIMG;
        IntPtr glRenderbufferStorageMultisampleIMG;
        IntPtr glFramebufferTexture2DMultisampleIMG;
        IntPtr glStartTilingQCOM;
        IntPtr glEndTilingQCOM;
        IntPtr evasglCreateSync;
        IntPtr evasglDestroySync;
        IntPtr evasglClientWaitSync;
        IntPtr evasglSignalSync;
        IntPtr evasglGetSyncAttrib;
        IntPtr evasglWaitSync;
        IntPtr evasglBindWaylandDisplay;
        IntPtr evasglUnbindWaylandDisplay;
        IntPtr evasglQueryWaylandBuffer;
        IntPtr glBeginQuery;
        IntPtr glBeginTransformFeedback;
        IntPtr glBindBufferBase;
        IntPtr glBindBufferRange;
        IntPtr glBindSampler;
        IntPtr glBindTransformFeedback;
        IntPtr glBindVertexArray;
        IntPtr glBlitFramebuffer;
        IntPtr glClearBufferfi;
        IntPtr glClearBufferfv;
        IntPtr glClearBufferiv;
        IntPtr glClearBufferuiv;
        IntPtr glClientWaitSync;
        IntPtr glCompressedTexImage3D;
        IntPtr glCompressedTexSubImage3D;
        IntPtr glCopyBufferSubData;
        IntPtr glCopyTexSubImage3D;
        IntPtr glDeleteQueries;
        IntPtr glDeleteSamplers;
        IntPtr glDeleteSync;
        IntPtr glDeleteTransformFeedbacks;
        IntPtr glDeleteVertexArrays;
        IntPtr glDrawArraysInstanced;
        IntPtr glDrawBuffers;
        IntPtr glDrawElementsInstanced;
        IntPtr glDrawRangeElements;
        IntPtr glEndQuery;
        IntPtr glEndTransformFeedback;
        IntPtr glFenceSync;
        IntPtr glFlushMappedBufferRange;
        IntPtr glFramebufferTextureLayer;
        IntPtr glGenQueries;
        IntPtr glGenSamplers;
        IntPtr glGenTransformFeedbacks;
        IntPtr glGenVertexArrays;
        IntPtr glGetActiveUniformBlockiv;
        IntPtr glGetActiveUniformBlockName;
        IntPtr glGetActiveUniformsiv;
        IntPtr glGetBufferParameteri64v;
        IntPtr glGetBufferPointerv;
        IntPtr glGetFragDataLocation;
        IntPtr glGetInteger64i_v;
        IntPtr glGetInteger64v;
        IntPtr glGetIntegeri_v;
        IntPtr glGetInternalformativ;
        IntPtr glGetProgramBinary;
        IntPtr glGetQueryiv;
        IntPtr glGetQueryObjectuiv;
        IntPtr glGetSamplerParameterfv;
        IntPtr glGetSamplerParameteriv;
        IntPtr glGetStringi;
        IntPtr glGetSynciv;
        IntPtr glGetTransformFeedbackVarying;
        IntPtr glGetUniformBlockIndex;
        IntPtr glGetUniformIndices;
        IntPtr glGetUniformuiv;
        IntPtr glGetVertexAttribIiv;
        IntPtr glGetVertexAttribIuiv;
        IntPtr glInvalidateFramebuffer;
        IntPtr glInvalidateSubFramebuffer;
        IntPtr glIsQuery;
        IntPtr glIsSampler;
        IntPtr glIsSync;
        IntPtr glIsTransformFeedback;
        IntPtr glIsVertexArray;
        IntPtr glMapBufferRange;
        IntPtr glPauseTransformFeedback;
        IntPtr glProgramBinary;
        IntPtr glProgramParameteri;
        IntPtr glReadBuffer;
        IntPtr glRenderbufferStorageMultisample;
        IntPtr glResumeTransformFeedback;
        IntPtr glSamplerParameterf;
        IntPtr glSamplerParameterfv;
        IntPtr glSamplerParameteri;
        IntPtr glSamplerParameteriv;
        IntPtr glTexImage3D;
        IntPtr glTexStorage2D;
        IntPtr glTexStorage3D;
        IntPtr glTexSubImage3D;
        IntPtr glTransformFeedbackVaryings;
        IntPtr glUniform1ui;
        IntPtr glUniform1uiv;
        IntPtr glUniform2ui;
        IntPtr glUniform2uiv;
        IntPtr glUniform3ui;
        IntPtr glUniform3uiv;
        IntPtr glUniform4ui;
        IntPtr glUniform4uiv;
        IntPtr glUniformBlockBinding;
        IntPtr glUniformMatrix2x3fv;
        IntPtr glUniformMatrix3x2fv;
        IntPtr glUniformMatrix2x4fv;
        IntPtr glUniformMatrix4x2fv;
        IntPtr glUniformMatrix3x4fv;
        IntPtr glUniformMatrix4x3fv;
        IntPtr glUnmapBuffer;
        IntPtr glVertexAttribDivisor;
        IntPtr glVertexAttribI4i;
        IntPtr glVertexAttribI4iv;
        IntPtr glVertexAttribI4ui;
        IntPtr glVertexAttribI4uiv;
        IntPtr glVertexAttribIPointer;
        IntPtr glWaitSync;
        IntPtr glDispatchCompute;
        IntPtr glDispatchComputeIndirect;
        IntPtr glDrawArraysIndirect;
        IntPtr glDrawElementsIndirect;
        IntPtr glFramebufferParameteri;
        IntPtr glGetFramebufferParameteriv;
        IntPtr glGetProgramInterfaceiv;
        IntPtr glGetProgramResourceIndex;
        IntPtr glGetProgramResourceName;
        IntPtr glGetProgramResourceiv;
        IntPtr glGetProgramResourceLocation;
        IntPtr glUseProgramStages;
        IntPtr glActiveShaderProgram;
        IntPtr glCreateShaderProgramv;
        IntPtr glBindProgramPipeline;
        IntPtr glDeleteProgramPipelines;
        IntPtr glGenProgramPipelines;
        IntPtr glIsProgramPipeline;
        IntPtr glGetProgramPipelineiv;
        IntPtr glProgramUniform1i;
        IntPtr glProgramUniform2i;
        IntPtr glProgramUniform3i;
        IntPtr glProgramUniform4i;
        IntPtr glProgramUniform1ui;
        IntPtr glProgramUniform2ui;
        IntPtr glProgramUniform3ui;
        IntPtr glProgramUniform4ui;
        IntPtr glProgramUniform1f;
        IntPtr glProgramUniform2f;
        IntPtr glProgramUniform3f;
        IntPtr glProgramUniform4f;
        IntPtr glProgramUniform1iv;
        IntPtr glProgramUniform2iv;
        IntPtr glProgramUniform3iv;
        IntPtr glProgramUniform4iv;
        IntPtr glProgramUniform1uiv;
        IntPtr glProgramUniform2uiv;
        IntPtr glProgramUniform3uiv;
        IntPtr glProgramUniform4uiv;
        IntPtr glProgramUniform1fv;
        IntPtr glProgramUniform2fv;
        IntPtr glProgramUniform3fv;
        IntPtr glProgramUniform4fv;
        IntPtr glProgramUniformMatrix2fv;
        IntPtr glProgramUniformMatrix3fv;
        IntPtr glProgramUniformMatrix4fv;
        IntPtr glProgramUniformMatrix2x3fv;
        IntPtr glProgramUniformMatrix3x2fv;
        IntPtr glProgramUniformMatrix2x4fv;
        IntPtr glProgramUniformMatrix4x2fv;
        IntPtr glProgramUniformMatrix3x4fv;
        IntPtr glProgramUniformMatrix4x3fv;
        IntPtr glValidateProgramPipeline;
        IntPtr glGetProgramPipelineInfoLog;
        IntPtr glBindImageTexture;
        IntPtr glGetBooleani_v;
        IntPtr glMemoryBarrier;
        IntPtr glMemoryBarrierByRegion;
        IntPtr glTexStorage2DMultisample;
        IntPtr glGetMultisamplefv;
        IntPtr glSampleMaski;
        IntPtr glGetTexLevelParameteriv;
        IntPtr glGetTexLevelParameterfv;
        IntPtr glBindVertexBuffer;
        IntPtr glVertexAttribFormat;
        IntPtr glVertexAttribIFormat;
        IntPtr glVertexAttribBinding;
        IntPtr glVertexBindingDivisor;
        IntPtr glBlendBarrier;
        IntPtr glCopyImageSubData;
        IntPtr glDebugMessageControl;
        IntPtr glDebugMessageInsert;
        IntPtr glDebugMessageCallback;
        IntPtr glGetDebugMessageLog;
        IntPtr glPushDebugGroup;
        IntPtr glPopDebugGroup;
        IntPtr glObjectLabel;
        IntPtr glGetObjectLabel;
        IntPtr glObjectPtrLabel;
        IntPtr glGetObjectPtrLabel;
        IntPtr glEnablei;
        IntPtr glDisablei;
        IntPtr glBlendEquationi;
        IntPtr glBlendEquationSeparatei;
        IntPtr glBlendFunci;
        IntPtr glBlendFuncSeparatei;
        IntPtr glColorMaski;
        IntPtr glIsEnabledi;
        IntPtr glDrawElementsBaseVertex;
        IntPtr glDrawRangeElementsBaseVertex;
        IntPtr glDrawElementsInstancedBaseVertex;
        IntPtr glFramebufferTexture;
        IntPtr glPrimitiveBoundingBox;
        IntPtr glGetGraphicsResetStatus;
        IntPtr glReadnPixels;
        IntPtr glGetnUniformfv;
        IntPtr glGetnUniformiv;
        IntPtr glGetnUniformuiv;
        IntPtr glMinSampleShading;
        IntPtr glPatchParameteri;
        IntPtr glTexParameterIiv;
        IntPtr glTexParameterIuiv;
        IntPtr glGetTexParameterIiv;
        IntPtr glGetTexParameterIuiv;
        IntPtr glSamplerParameterIiv;
        IntPtr glSamplerParameterIuiv;
        IntPtr glGetSamplerParameterIiv;
        IntPtr glGetSamplerParameterIuiv;
        IntPtr glTexBuffer;
        IntPtr glTexBufferRange;
        IntPtr glTexStorage3DMultisample;
#pragma warning restore 0169
    }
}