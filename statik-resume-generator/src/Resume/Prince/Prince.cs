using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Resume.Prince
{
    
    public interface IPrince
    {
        void SetEncryptInfo(int keyBits,
                            string userPassword,
                            string ownerPassword,
                            bool disallowPrint,
                            bool disallowModify,
                            bool disallowCopy,
                            bool disallowAnnotate);
        void AddStyleSheet(string cssPath);
        void ClearStyleSheets();
        void AddScript(string jsPath);
        void ClearScripts();
        void AddFileAttachment(string filePath);
        void ClearFileAttachments();
        void SetLicenseFile(string file);
        void SetLicensekey(string key);
        void SetInputType(string inputType);
        void SetHTML(bool html);
        void SetJavaScript(bool js);
        void SetNoNetwork(bool noNetwork);
        void SetHttpProxy(string proxy);
        void SetHttpTimeout(string httpTimeout);
        void SetCookie(string cookie);
        void SetCookieJar(string cookieJar);
        void SetSslCaCert(string sslCaCert);
        void SetSslCaPath(string sslCaPath);
        void SetSslVersion(string sslVersion);
        void SetInsecure(bool insecure);
        void SetNoParallelDownloads(bool noParallelDownloads);
        void SetLog(string logFile);
        void SetVerbose(bool verbose);
        void SetDebug(bool debug);
        void SetNoWarnCss(bool noWarnCss);
        void SetBaseURL(string baseurl);
        void SetFileRoot(string fileroot);
        void SetXInclude(bool xInclude);
        void SetXmlExternalEntities(bool xmlExternalEntities);
        void AddRemap(string url, string dir);
        void ClearRemaps();
        void SetEmbedFonts(bool embed);
        void SetSubsetFonts(bool embedSubset);
        void SetForceIdentityEncoding(bool forceIdentityEncoding);
        void SetCompress(bool compress);
        void SetPDFOutputIntent(string pdfOutputIntent, bool convertColors = false);
        void SetPDFProfile(string pdfProfile);
        void SetNoArtificialFonts(bool noArtificialFonts);
        void SetFallbackCmykProfile(string fallbackCmykProfile);
        void SetPDFTitle(string pdfTitle);
        void SetPDFSubject(string pdfSubject);
        void SetPDFAuthor(string pdfAuthor);
        void SetPDFKeywords(string keywords);
        void SetPDFCreator(string creator);
        void SetAuthMethod(string authMethod);
        void SetAuthUser(string user);
        void SetAuthPassword(string password);
        void SetAuthServer(string server);
        void SetAuthScheme(string scheme);
        void SetNoAuthPreemptive(bool noAuthPreemtive);
        void SetMedia(string media);
        void SetPageSize(string pageSize);
        void SetPageMargin(string pageMargin);
        void SetNoAuthorStyle(bool noAuthorStyle);
        void SetNODefaultStyle(bool noDefaultStyle);
        void SetEncrypt(bool encrypt);
        void SetOptions(string options);
        bool Convert(Stream xmlInput, Stream pdfOutput, List<Tuple<string, string>> dats = null);
        bool Convert(Stream xmlInput, string pdfPath, List<Tuple<string, string>> dats = null);
        bool Convert(string xmlPath, Stream pdfOutput, List<Tuple<string, string>> dats = null);
        bool Convert(string xmlPath, string pdfPath, List<Tuple<string, string>> dats = null);
        bool Convert(string xmlPath, List<Tuple<string, string>> dats = null);
        bool ConvertMemoryStream(MemoryStream xmlInput, Stream pdfOutput, List<Tuple<string, string>> dats = null);
        bool ConvertMultiple(string[] xmlPaths, string pdfPath, List<Tuple<string, string>> dats = null);
        bool ConvertString(string xmlInput, Stream pdfOutput, List<Tuple<string, string>> dats = null);
    }

    public interface PrinceEvents
    {
        void onMessage(string msgType, string msgLocation, string msgText);
    }


    public class PrinceFilter : Stream
    {
        private readonly IPrince mPrince;
        private readonly Stream mOldFilter;
        private readonly MemoryStream mMemStream;
    
        public PrinceFilter(IPrince prince, Stream oldFilter)
        {
            mPrince = prince;
            mOldFilter = oldFilter;
            mMemStream = new MemoryStream();
        }
    
        public override bool CanSeek
        {
            get { return false; }
        }
    
        public override bool CanWrite
        {
            get { return true; }
        }
    
        public override bool CanRead
        {
            get { return false; }
        }
    
        public override long Position
        {
            get
            {
                return 0;
            }
    
            set
            {
                /* Do Nothing */
            }
        }
    
        public override long Length
        {
            get
            {
                return 0;
            }
        }
    
        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }
    
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }
    
        public override void SetLength(long value)
        {
            /* Do Nothing */
        }
    
        public override void Write(byte[] buffer, int offset, int count)
        {
            mMemStream.Write(buffer, offset, count);
        }
    
        public override void Flush()
        {
            /* FIXME? */
        }
    
        public override void Close()
        {
            mPrince.ConvertMemoryStream(mMemStream, mOldFilter);
            mOldFilter.Close();
        }
    }


    public class Prince : IPrince
    {
        private PrinceEvents mEvents;
        private readonly string mPrincePath;
        protected string mStyleSheets;
        protected string mJavaScripts;
        
        private string mLicenseFile;
        private string mLicenseKey;
    
        //Input settings
        protected string mInputType;
        protected string mBaseURL;
        private string mFileRoot;
        protected bool mJavaScript; 
        protected bool mXInclude;
        protected bool mXmlExternalEntities;
        protected bool mNoLocalFiles;
        private string mRemaps;
    
        //Network settings
        private bool mNoNetwork;
        private string mHttpProxy;
        private string mHttpTimeout;
        private string mCookie;
        private string mCookieJar;
        private string mSslCaCert;
        private string mSslCaPath;
        private string mSslVersion;
        private bool mInsecure;
        private bool mNoParallelDownloads;
        private string mAuthMethod;
        private string mAuthUser;
        private string mAuthPassword;
        private string mAuthServer;
        private string mAuthScheme;
        private bool mNoAuthPreemptive;
    
        //Log Settings
        private string mLogFile;
        private bool mVerbose;
        private bool mDebug;
        private bool mNoWarnCss;
        
        //PDF settings
        protected string mPDFOutputIntent;
        protected string mPDFProfile;
        protected string mFileAttachments;
        protected bool mNoArtificialFonts;
        protected bool mEmbedFonts;
        protected bool mSubsetFonts;
        protected bool mForceIdentityEncoding;
        protected bool mCompress;
        protected bool mConvertColors;
        protected string mFallbackCmykProfile;
    
        protected string mPdfTitle;
        protected string mPdfSubject;
        protected string mPdfAuthor;
        protected string mPdfKeywords;
        protected string mPdfCreator;
    
    
        //CSS settings
        protected string mMedia;
        private string mPageSize;
        private string mPageMargin;
        protected bool mNoAuthorStyle;
        protected bool mNoDefaultStyle;
    
        //Encryption settings
        protected bool mEncrypt;
        protected int mKeyBits;
        protected string mUserPassword;
        protected string mOwnerPasssword;
        protected bool mDisallowPrint;
        protected bool mDisallowModify;
        protected bool mDisallowCopy;
        protected bool mDisallowAnnotate;
    
        //Other command-line options
        private string mOptions;
    
    
        public Prince()
        {
            mEvents = null;
            mInputType = "auto";
            mJavaScript = false;
            mNoNetwork = false;
            mVerbose = false;
            mDebug = false;
            mNoWarnCss = false;
            mInsecure = false;
            mNoParallelDownloads = false;
            mXInclude = false;
            mXmlExternalEntities = false;
            mNoLocalFiles = false;
            mEmbedFonts = true;
            mSubsetFonts = true;
            mForceIdentityEncoding = false;
            mCompress = true;
            mConvertColors = false;
            mNoArtificialFonts = false;
            mNoAuthPreemptive = false;
            mNoAuthorStyle = false;
            mNoDefaultStyle = false;
            mEncrypt = false;
            mKeyBits = 40;
            mDisallowPrint = false;
            mDisallowModify = false;
            mDisallowCopy = false;
            mDisallowAnnotate = false;
            mStyleSheets = "";
            mJavaScripts = "";
            mFileAttachments = "";
            mRemaps = "";
        }
    
        public Prince(string princePath) : this()
        {
            mPrincePath = princePath;
        }
    
        public Prince(string princePath, PrinceEvents events) : this()
        {
            mPrincePath = princePath;
            mEvents = events;
        }
    
        #region Public methods
    
        public void AddStyleSheet(string cssPath)
        {
            mStyleSheets += "-s \"" + escape(cssPath) + "\" ";
        }
    
        public void ClearStyleSheets()
        {
            mStyleSheets = "";
        }
    
        public void AddScript(string jsPath)
        {
            mJavaScripts += "--script \"" + escape(jsPath) + "\" " ;
        }
    
        public void ClearScripts()
        {
            mJavaScripts = "";
        }
    
        public void AddFileAttachment(string filePath)
        {
            mFileAttachments += "--attach=\"" +  escape(filePath) + "\" ";
        }
    
        public void ClearFileAttachments()
        {
            mFileAttachments = "";
        }
        
        public void SetLicenseFile(string file)
        {
            mLicenseFile = file;
        }
    
        public void SetLicensekey(string key)
        {
            mLicenseKey = key;
        }
    
        public void SetInputType(string inputType)
        {
            mInputType = inputType;
        }
    
        public void SetHTML(bool html)
        {
            mInputType = (html ? "html" : "xml");
        }
    
        public void SetJavaScript(bool js)
        {
            mJavaScript = js;
        }
    
        public void SetNoNetwork(bool noNetwork)
        {
            mNoNetwork = noNetwork;
        }
    
        public void SetHttpProxy(string proxy)
        {
            mHttpProxy = proxy;
        }
    
        public void SetHttpTimeout(string httpTimeout)
        {
            mHttpTimeout = httpTimeout;
        }
    
        public void SetCookie(string cookie)
        {
            mCookie = cookie;
        }
    
        public void SetCookieJar(string cookieJar)
        {
            mCookieJar = cookieJar;
        }
    
        public void SetSslCaCert(string sslCaCert)
        {
            mSslCaCert = sslCaCert;
        }
    
        public void SetSslCaPath(string sslCaPath)
        {
            mSslCaPath = sslCaPath;
        }
    
        public void SetSslVersion(string sslVersion)
        {
            mSslVersion = sslVersion;
        }
    
        public void SetInsecure(bool insecure)
        {
            mInsecure = insecure;
        }
    
        public void SetNoParallelDownloads(bool noParallelDownloads)
        {
            mNoParallelDownloads = noParallelDownloads;
        }
    
        public void SetLog(string logFile)
        {
            mLogFile = logFile;
        }
    
        public void SetVerbose(bool verbose)
        {
            mVerbose = verbose;
        }
    
        public void SetDebug(bool debug)
        {
            mDebug = debug;
        }
    
        public void SetNoWarnCss(bool noWarnCss)
        {
            mNoWarnCss = noWarnCss;
        }
    
        public void SetBaseURL(string baseURL)
        {
            mBaseURL = baseURL;
        }
    
        public void SetFileRoot(string fileRoot)
        {
            mFileRoot = fileRoot;
        }
    
        public void SetXInclude(bool xInclude)
        {
            mXInclude = xInclude;
        }
    
        public void SetXmlExternalEntities(bool xmlExternalEntities)
        {
            mXmlExternalEntities = xmlExternalEntities;
        }
    
        public void SetNoLocalFiles(bool noLocalFiles)
        {
            mNoLocalFiles = noLocalFiles;
        }
    
        public void AddRemap(string url, string dir)
        {
            mRemaps += "--remap=\"" + escape(url) + "\"=\"" + escape(dir) + "\" ";
        }
    
        public void ClearRemaps()
        {
            mRemaps = "";
        }
    
        public void SetEmbedFonts(bool embed)
        {
            mEmbedFonts = embed;
        }
    
        public void SetSubsetFonts(bool subset)
        {
            mSubsetFonts = subset;
        }
    
        public void SetForceIdentityEncoding(bool forceIdentityEncoding)
        {
            mForceIdentityEncoding = forceIdentityEncoding;
        }
    
        public void SetCompress(bool compress)
        {
            mCompress = compress;
        }
    
        public void SetPDFOutputIntent(string pdfOutputIntent, bool convertColors = false)
        {
            mPDFOutputIntent = pdfOutputIntent;
            mConvertColors = convertColors;
        }
    
        public void SetPDFProfile(string pdfProfile)
        {
            mPDFProfile = pdfProfile;
        }
    
        public void SetNoArtificialFonts(bool noArtificialFonts)
        {
            mNoArtificialFonts = noArtificialFonts;
        }
    
        public void SetFallbackCmykProfile(string fallbackCmykProfile)
        {
            mFallbackCmykProfile = fallbackCmykProfile;
        }
    
        public void SetPDFTitle(string pdfTitle)
        {
            mPdfTitle = pdfTitle;
        }
    
        public void SetPDFSubject(string pdfSubject)
        {
            mPdfSubject = pdfSubject;
        }
    
        public void SetPDFAuthor(string pdfAuthor)
        {
            mPdfAuthor = pdfAuthor;
        }
    
        public void SetPDFKeywords(string pdfKeywords)
        {
            mPdfKeywords = pdfKeywords;
        }
    
        public void SetPDFCreator(string pdfCreator)
        {
            mPdfCreator = pdfCreator;
        }
    
        public void SetAuthMethod(string authMethod)
        {
            if (String.Equals(authMethod, "basic", StringComparison.OrdinalIgnoreCase))
            {
                mAuthMethod = "basic";
            }
            else if (String.Equals(authMethod, "digest", StringComparison.OrdinalIgnoreCase))
            {
                mAuthMethod = "digest";
            }
            else if (String.Equals(authMethod, "ntlm", StringComparison.OrdinalIgnoreCase))
            {
                mAuthMethod = "ntlm";
            }
            else if (String.Equals(authMethod, "negotiate", StringComparison.OrdinalIgnoreCase))
            {
                mAuthMethod = "negotiate";
            }
            else
            {
                mAuthMethod = "";
            }
        }
    
        public void SetAuthUser(string authUser)
        {
            mAuthUser = authUser;
        }
    
        public void SetAuthPassword(string authPassword)
        {
            mAuthPassword = authPassword;
        }
    
        public void SetAuthServer(string authServer)
        {
            mAuthServer = authServer;
        }
    
        public void SetAuthScheme(string authScheme)
        {
            if (String.Equals(authScheme, "http", StringComparison.OrdinalIgnoreCase))
            {
                mAuthScheme = "http";
            }
            else if (String.Equals(authScheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                mAuthScheme = "https";
            }
            else
            {
                mAuthScheme = "";
            }
        }
    
        public void SetNoAuthPreemptive(bool noAuthPreemptive)
        {
            mNoAuthPreemptive = noAuthPreemptive;
        }
    
        public void SetMedia(string media)
        {
            mMedia = media;
        }
    
        public void SetPageSize(string pageSize)
        {
            mPageSize = pageSize;
        }
    
        public void SetPageMargin(string pageMargin)
        {
            mPageMargin = pageMargin;
        }
    
        public void SetNoAuthorStyle(bool noAuthorStyle)
        {
            mNoAuthorStyle = noAuthorStyle;
        }
    
        public void SetNODefaultStyle(bool noDefaultStyle)
        {
            mNoDefaultStyle = noDefaultStyle;
        }
    
        public void SetEncrypt(bool encrypt)
        {
            mEncrypt = encrypt;
        }
    
        public void SetEncryptInfo(int keyBits, 
                                   string userPassword, 
                                   string ownerPassword, 
                                   bool disallowPrint, 
                                   bool disallowModify, 
                                   bool disallowCopy, 
                                   bool disallowAnnotate)
        {
          
            if ((keyBits != 40) && (keyBits != 128))
            {
                throw new ApplicationException("Invalid value for keyBits: " + keyBits.ToString() + " (must be 40 or 128)");
            }
    
    
            mEncrypt = true;
            mKeyBits = keyBits;
            mUserPassword = userPassword;
            mOwnerPasssword = ownerPassword;
            mDisallowPrint = disallowPrint;
            mDisallowModify = disallowModify;
            mDisallowCopy = disallowCopy;
            mDisallowAnnotate = disallowAnnotate;
    
        }
    
        public void SetOptions(string options)
        {
            mOptions = options;
        }
    
    
    
        public bool Convert(string xmlPath, List<Tuple<string, string>> dats = null)
        {
            string args = getArgs("normal") + "\"" + escape(xmlPath) + "\"";
            return Convert1(args, dats);
        }
    
        public bool Convert(string xmlPath, string pdfPath, List<Tuple<string, string>> dats = null)
        {
            string args = getArgs("normal") + "\"" + escape(xmlPath) + "\" -o \"" + escape(pdfPath) + "\"";
            return Convert1(args, dats);
        }
    
        public bool ConvertMultiple(string[] xmlPaths, string pdfPath, List<Tuple<string, string>> dats = null)
        {
            string docPaths = "";
    
            for (int i = 0; i < xmlPaths.Length; i++)
            {
                docPaths +=  "\"" + escape(xmlPaths[i]) + "\" ";
            }
    
            string args = getArgs("normal") + docPaths + " -o \"" + escape(pdfPath) + "\"";
    
            return Convert1(args, dats);
        }
    
        public bool Convert(string xmlPath, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            if (!pdfOutput.CanWrite)
                throw new ApplicationException("The pdfOutput stream is not writable");
    
            var buf = new byte[4096];
            string args = getArgs("buffered") + "\"" + escape(xmlPath) + "\" -o -" ;
            Process prs = StartPrince(args);
            prs.StandardInput.Close();
    
            int bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
    
            while (bytesRead != 0)
            {
                pdfOutput.Write(buf, 0, bytesRead);
                bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            }
    
            prs.StandardOutput.Close();
            return (ReadMessages(prs.StandardError, dats) == "success");
        }
    
        public bool Convert(Stream xmlInput, string pdfPath, List<Tuple<string, string>> dats = null)
        {
            if (!xmlInput.CanRead)
                throw new ApplicationException("The xmlInput stream is not readable");
    
            var buf = new byte[4096];
    
            string args = getArgs("buffered") + "- -o \"" + escape(pdfPath) + "\"";
            Process prs = StartPrince(args);
    
            int bytesRead = xmlInput.Read(buf, 0, 4096);
            while (bytesRead != 0)
            {
                prs.StandardInput.BaseStream.Write(buf, 0, bytesRead);
                bytesRead = xmlInput.Read(buf, 0, 4096);
            }
    
            prs.StandardInput.Close();
            prs.StandardOutput.Close();
    
            return (ReadMessages(prs.StandardError, dats) == "success");
        }
    
        public bool Convert(Stream xmlInput, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            if (!xmlInput.CanRead)
                throw new ApplicationException("The xmlInput stream is not readable");
    
            if (!pdfOutput.CanWrite)
                throw new ApplicationException("The pdfOutput stream is not writable");
    
            var buf = new byte[4096];
    
            string args = getArgs("buffered") + "-";
            Process prs = StartPrince(args);
    
            int bytesRead = xmlInput.Read(buf, 0, 4096);
            while (bytesRead != 0)
            {
                prs.StandardInput.BaseStream.Write(buf, 0, bytesRead);
                bytesRead = xmlInput.Read(buf, 0, 4096);
            }
            prs.StandardInput.Close();
    
            bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            while (bytesRead != 0)
            {
                pdfOutput.Write(buf, 0, bytesRead);
                bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            }
            prs.StandardOutput.Close();
    
            return (ReadMessages(prs.StandardError, dats) == "success");
        }
    
        public bool ConvertMemoryStream(MemoryStream xmlInput, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            if (!pdfOutput.CanWrite)
                throw new ApplicationException("The pdfOutput stream is not writable");
    
            var buf = new byte[4096];
    
            string args = getArgs("buffered") + "-";
            Process prs = StartPrince(args);
    
            xmlInput.WriteTo(prs.StandardInput.BaseStream);
            prs.StandardInput.Close();
    
            int bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            while (bytesRead != 0)
            {
                pdfOutput.Write(buf, 0, bytesRead);
                bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            }
            prs.StandardOutput.Close();
    
            return (ReadMessages(prs.StandardError, dats) == "success");
        }
    
        public bool ConvertString(string xmlInput, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            if (!pdfOutput.CanWrite)
                throw new ApplicationException("The pdfOutput stream is not writable");
    
            var buf = new byte[4096];
    
            string args = getArgs("buffered") + "-";
            Process prs = StartPrince(args);
    
            var enc = new UTF8Encoding();
            byte[] stringBytes = enc.GetBytes(xmlInput);
            prs.StandardInput.BaseStream.Write(stringBytes, 0, stringBytes.Length);
            prs.StandardInput.Close();
    
            int bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            while (bytesRead != 0)
            {
                pdfOutput.Write(buf, 0, bytesRead);
                bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
            }
            prs.StandardOutput.Close();
    
            return (ReadMessages(prs.StandardError, dats) == "success");
        }
    
    
        #endregion
    
        #region Private methods
    
        private string getArgs(string logType)
        {
            string args = "--structured-log=" + logType + " " + mStyleSheets + mJavaScripts + mFileAttachments + mRemaps;
    
            args += getBaseCommandLine();
    
            args += getJobCommandLine();
    
            return args;
        }
    
        protected string getBaseCommandLine()
        {
            string baseCommandLine = "";
    
            if (!string.IsNullOrEmpty(mFileRoot)) { baseCommandLine += "--fileroot=\"" + escape(mFileRoot) + "\" "; }
            if (mNoNetwork) { baseCommandLine += "--no-network "; }
            if (!string.IsNullOrEmpty(mAuthMethod)) { baseCommandLine += "--auth-method=\"" + escape(mAuthMethod) + "\" "; }
            if (!string.IsNullOrEmpty(mAuthUser)) { baseCommandLine += "--auth-user=\"" + escape(mAuthUser) + "\" "; }
            if (!string.IsNullOrEmpty(mAuthPassword)) { baseCommandLine += "--auth-password=\"" + escape(mAuthPassword) + "\" "; }
            if (!string.IsNullOrEmpty(mAuthServer)) { baseCommandLine += "--auth-server=\"" + escape(mAuthServer) + "\" "; }
            if (!string.IsNullOrEmpty(mAuthScheme)) { baseCommandLine += "--auth-scheme=\"" + escape(mAuthScheme) + "\" "; }
            if (mNoAuthPreemptive) { baseCommandLine += "--no-auth-preemptive "; }
            if (!string.IsNullOrEmpty(mHttpProxy)) { baseCommandLine += "--http-proxy=\"" + escape(mHttpProxy) + "\" "; }
            if (!string.IsNullOrEmpty(mHttpTimeout)) { baseCommandLine += "--http-timeout=\"" + escape(mHttpTimeout) + "\" "; }
            if (!string.IsNullOrEmpty(mCookie)) { baseCommandLine += "--cookie=\"" + escape(mCookie) + "\" "; }
            if (!string.IsNullOrEmpty(mCookieJar)) { baseCommandLine += "--cookiejar=\"" + escape(mCookieJar) + "\" "; }
            if (!string.IsNullOrEmpty(mSslCaCert)) { baseCommandLine += "--ssl-cacert=\"" + escape(mSslCaCert) + "\" "; }
            if (!string.IsNullOrEmpty(mSslCaPath)) { baseCommandLine += "--ssl-capath=\"" + escape(mSslCaPath) + "\" "; }
            if (!string.IsNullOrEmpty(mSslVersion)) { baseCommandLine += "--ssl-version=\"" + escape(mSslVersion) + "\" "; }
            if (mInsecure) { baseCommandLine += "--insecure "; }
            if (mNoParallelDownloads) { baseCommandLine += "--no-parallel-downloads "; }
            if (!string.IsNullOrEmpty(mLogFile)) { baseCommandLine += "--log=\"" + escape(mLogFile) + "\" "; }
            if (mVerbose) { baseCommandLine += "--verbose "; }
            if (mDebug) { baseCommandLine += "--debug "; }
            if (mNoWarnCss) { baseCommandLine += "--no-warn-css "; }
    
            return baseCommandLine;
        }
    
        protected string getJobCommandLine()
        {
            string jobCommandLine = "";
    
            
            if (!string.IsNullOrEmpty(mInputType) && !mInputType.Equals("auto")) { jobCommandLine +=  "--input=\"" + escape(mInputType) + "\" "; }
            if (mJavaScript) { jobCommandLine += "--javascript "; }
            if (!string.IsNullOrEmpty(mBaseURL)) { jobCommandLine += "--baseurl=\"" + escape(mBaseURL) + "\" "; }
            if (!string.IsNullOrEmpty(mLicenseFile)) { jobCommandLine += "--license-file=\"" + escape(mLicenseFile) + "\" "; }
            if (!string.IsNullOrEmpty(mLicenseKey)) { jobCommandLine += "--license-key=\"" + escape(mLicenseKey) + "\" "; }
            if (mXInclude) { jobCommandLine += "--xinclude "; }
            if (!mXInclude) { jobCommandLine += "--no-xinclude "; }
            if (mXmlExternalEntities) { jobCommandLine += "--xml-external-entities "; }
            if (mNoLocalFiles) { jobCommandLine += "--no-local-files "; }
            if (!mEmbedFonts) { jobCommandLine += "--no-embed-fonts "; }
            if (!mSubsetFonts) { jobCommandLine += "--no-subset-fonts "; }
            if (mForceIdentityEncoding) {jobCommandLine += "--force-identity-encoding "; }
            if (!mCompress) { jobCommandLine += "--no-compress "; }
            if (mEncrypt)
            {
                jobCommandLine += "--encrypt --key-bits " + mKeyBits.ToString() + " ";
                if(!string.IsNullOrEmpty(mUserPassword)) { jobCommandLine += "--user-password=\"" + escape(mUserPassword) + "\" "; }
                if(!string.IsNullOrEmpty(mOwnerPasssword)) { jobCommandLine += "--owner-password=\"" + escape(mOwnerPasssword) + "\" "; }
                if(mDisallowPrint) { jobCommandLine += "--disallow-print "; }
                if(mDisallowModify) { jobCommandLine += "--disallow-modify "; }
                if(mDisallowCopy) { jobCommandLine += "--disallow-copy "; }
                if(mDisallowAnnotate) { jobCommandLine += "--disallow-annotate "; }
            }
    
            if (!string.IsNullOrEmpty(mPDFProfile)) { jobCommandLine += "--pdf-profile=\"" + escape(mPDFProfile) + "\" "; }
            if (!string.IsNullOrEmpty(mPDFOutputIntent)) 
            { 
                jobCommandLine += "--pdf-output-intent=\"" + escape(mPDFOutputIntent) + "\" ";
                if (mConvertColors)
                {
                    jobCommandLine += "--convert-colors ";
                }
            }
            if (mNoArtificialFonts) { jobCommandLine += "--no-artificial-fonts "; }
            if (!string.IsNullOrEmpty(mFallbackCmykProfile)) { jobCommandLine += "--fallback-cmyk-profile=\"" + escape(mFallbackCmykProfile) + "\" "; }
            if (!string.IsNullOrEmpty(mPdfTitle)) { jobCommandLine += "--pdf-title=\"" + escape(mPdfTitle) + "\" "; }
            if (!string.IsNullOrEmpty(mPdfSubject)) { jobCommandLine += "--pdf-subject=\"" + escape(mPdfSubject) + "\" "; }
            if (!string.IsNullOrEmpty(mPdfAuthor)) { jobCommandLine += "--pdf-author=\"" + escape(mPdfAuthor) + "\" "; }
            if (!string.IsNullOrEmpty(mPdfKeywords)) { jobCommandLine += "--pdf-keywords=\"" + escape(mPdfKeywords) + "\" "; }
            if (!string.IsNullOrEmpty(mPdfCreator)) { jobCommandLine += "--pdf-creator=\"" + escape(mPdfCreator) + "\" "; }
            if (!string.IsNullOrEmpty(mMedia)) { jobCommandLine += "--media=\"" + escape(mMedia) + "\" "; }
            if (!string.IsNullOrEmpty(mPageSize)) { jobCommandLine += "--page-size=\"" + escape(mPageSize) + "\" "; }
            if (!string.IsNullOrEmpty(mPageMargin)) { jobCommandLine += "--page-margin=\"" + escape(mPageMargin) + "\" "; }
            if (mNoAuthorStyle) { jobCommandLine += "--no-author-style "; }
            if (mNoDefaultStyle) { jobCommandLine += "--no-default-style "; }
            if(!string.IsNullOrEmpty(mOptions)) {jobCommandLine += escape(mOptions) + " ";}
    
    
            return jobCommandLine;
        }
    
        private bool Convert1(string args, List<Tuple<string, string>> dats)
        {
            Process pr = StartPrince(args);
            return (pr != null && ReadMessages(pr.StandardError, dats) == "success");
        }
    
        protected Process StartPrince(string args)
        {
            const int ERROR_FILE_NOT_FOUND = 2;
            const int ERROR_PATH_NOT_FOUND = 3;
            const int ERROR_ACCESS_DENIED = 5;
    
            var pr = new Process
            {
                StartInfo =
                {
                    FileName = mPrincePath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
    
            try
            {
                pr.Start();
    
                if (!pr.HasExited)
                {
                    return pr;
                }
    
                throw new ApplicationException("Error starting Prince: " + mPrincePath);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                /* By default, use ex.Message */
                string msg = ex.Message;
    
                switch (ex.NativeErrorCode)
                {
                    case ERROR_FILE_NOT_FOUND:
                        msg += " -- Please verify that Prince.exe is in the directory";
                        break;
                    case ERROR_ACCESS_DENIED:
                        msg += " -- Please check system permission to run Prince.";
                        break;
                    case ERROR_PATH_NOT_FOUND:
                        msg += " -- Please check Prince path.";
                        break;
                }
    
                throw new ApplicationException(msg);
            }
        }
    
    
        protected string ReadMessages(StreamReader strRdr, List<Tuple<string, string>> dats)
        {
            //StreamReader stdErrFromPr = prs.StandardError;
            string line = strRdr.ReadLine();
            string result = "";
    
    
            while (line != null)
            {
                if (line.Length >= 4)
                {
                    string msgTag = line.Substring(0, 4);
                    string msgBody = line.Substring(4);
    
                    if((mEvents != null) && msgTag.Equals("msg|"))
                    {
                       handleMessage(msgBody);
                    }
                    else if ((dats != null) && msgTag.Equals("dat|"))
                    {
                        string delimStr = "|";
                        char[] delimiter = delimStr.ToCharArray();
                        string[] dataParts = msgBody.Split(delimiter, 2);
    
                        dats.Add(new Tuple<string, string>(dataParts[0], dataParts[1]));
                    }
                    else if (msgTag.Equals("fin|"))
                    {
                        result = msgBody;
                    }
                    else
                    {
                        // ignore unknown log messages
                    }
    
                    line = strRdr.ReadLine();
                }
                else
                {
                    //ignore too short messages
                }
            }
    
            strRdr.Close();
    
            return result;
        }
    
        private void handleMessage(string msgBody)
        {
            if (msgBody.Length >= 4)
            {
                string msgType = msgBody.Substring(0, 3);
                string tmpStr = msgBody.Substring(4);
    
                int locoffset = tmpStr.IndexOf('|');
    
                if (locoffset != -1)
                {
                    string msgLocation = tmpStr.Substring(0, locoffset);
                    string msgText = tmpStr.Substring(locoffset + 1);
    
                    mEvents.onMessage(msgType, msgLocation, msgText);
                }
                else
                {
                    // ignore incorrectly formatted messages
                }
            }
            else
            {
                // ignore too short messages
            }
        }
    
        private static string cmdline_arg_escape_1(string arg)
        {
            if (arg.Length == 0)
                return arg; /* return empty string */
    
            //chr(34) is character double quote ( " ), chr(92) is character backslash ( \ )
            for (var pos = arg.Length - 1; pos >= 0; pos--)
            {
                if (arg[pos] == (char)34)
                {
                    //if there is a double quote in the arg string
                    //find number of backslashes preceding the double quote ( " )
                    int numSlashes = 0;
    
                    while ((pos - 1 - numSlashes) >= 0)
                    {
                        if (arg[pos - 1 - numSlashes] == (char)92)
                            numSlashes += 1;
                        else
                            break;
                    }
    
                    string rightSubstring = arg.Substring(pos + 1);
                    string leftSubstring = arg.Substring(0, (pos - numSlashes));
                    string middleSubstring = ((char)92).ToString(CultureInfo.InvariantCulture);
    
                    for (var i = 1; i <= numSlashes; i++)
                        middleSubstring = middleSubstring + (char)92 + (char)92;
    
                    middleSubstring = middleSubstring + (char)34;
                    return cmdline_arg_escape_1(leftSubstring) + middleSubstring + rightSubstring;
                }
            }
    
            //no double quote found, return string itself
            return arg;
        }
    
        private static string cmdline_arg_escape_2(string arg)
        {
            int numEndingSlashes = 0;
    
            for (var pos = arg.Length - 1; pos >= 0; pos--)
            {
                if (arg[pos] == (char)92)
                    numEndingSlashes += 1;
                else
                    break;
            }
    
            for (var i = 1; i <= numEndingSlashes; i++)
                arg = arg + (char)92;
    
            return arg;
        }
    
        protected string escape(string arg)
        {
            return cmdline_arg_escape_2(cmdline_arg_escape_1(arg));
        }
    
        #endregion
    
    }


    public class Chunk
    {
        private string mTag;
        private byte[] mBytes;
    
        private Chunk(string tag, byte[] bytes)
        {
            mTag = tag;
            mBytes = bytes;
        }
    
        public string GetTag()
        {
            return mTag;
        }
    
        public byte[] GetBytes()
        {
            return mBytes;
        }
    
        public string GetString()
        {
            return System.Text.Encoding.UTF8.GetString(mBytes);
        }
    
        public StreamReader getReader()
        {
            return new StreamReader(new MemoryStream(mBytes));
        }
    
        public static Chunk readChunk(StreamReader input)
        {
            byte[] tagBytes = new byte[3];
    
            if (input.BaseStream.Read(tagBytes, 0, 3) != 3)
            {
                throw new IOException("failed to read chunk tag");
            }
    
            string tag = System.Text.Encoding.ASCII.GetString(tagBytes);
    
            byte[] b = new byte[1];
            
            if(input.BaseStream.Read(b, 0, 1) != 1)
            {
                throw new IOException("failed to read byte after chunk tag");
            }
    
            if (b[0] != ' ')
            {
                throw new IOException("missing space after chunk tag");
            }
    
            int length = 0;
            int maxNumLength = 9;
            int numLength = 0;
    
            for (; numLength < maxNumLength+1; numLength++)
            {
                input.BaseStream.Read(b, 0, 1);
    
                if (b[0] == '\n') break;
    
                if (b[0] < '0' || b[0] > '9')
                    throw new IOException("unexpected character in chunk length");
    
                length *= 10;
                length += b[0] - '0';
            }
    
            if (numLength < 1 || numLength > maxNumLength)
                throw new IOException("invalid chunk length");
    
            byte[] bytes = new byte[length];
    
            int offset = 0;
    
            while (length > 0)
            {
                int count = input.BaseStream.Read(bytes, offset, length);
    
                if (count < 0)
                    throw new IOException("failed to read chunk data");
    
                if (count > length)
                    throw new IOException("unexpected read overrun");
    
                length -= count;
                offset += count;
            }
    
            input.BaseStream.Read(b, 0, 1);
    
            if (b[0] != '\n')
                throw new IOException("missing newline after chunk data");
    
            return new Chunk(tag, bytes);
        }
    
        public static void writeChunk(Stream output, string tag, string data)
        {
            writeChunk(output, tag, Encoding.UTF8.GetBytes(data));
        }
    
        public static void writeChunk(Stream output, string tag, byte[] data)
        {
            string s = tag + " " + data.Length + "\n";
    
            if (!output.CanWrite)
                throw new ApplicationException("The output stream is not writable");
    
            byte[] b = Encoding.UTF8.GetBytes(s);
    
            output.Write(b, 0, b.Length);
            output.Write(data, 0, data.Length);
    
            byte[] nl = Encoding.UTF8.GetBytes("\n");
            output.Write(nl, 0, nl.Length);
    
        }
    
    }

    public class PrinceControl : Prince
    {
        private Process mProcess;
        private string mVersion;
        private List<byte[]> jobResources;
        private List<string> documents;
        private List<Attachment> attachments;
    
        private class Attachment
        {
            public string url { get; set; }
            public string filename { get; set; }
            public string description { get; set; }
        }
    
        /** Constructor for PrinceControl.
         * @param exePath is the path of the Prince executable.
         */
        public PrinceControl(string exePath)
            : base(exePath)
        {
            jobResources = new List<byte[]>();
            documents = new List<string>();
            attachments = new List<Attachment>();
        }
    
        /** Constructor for PrinceControl.
         * @param exePath The path of the Prince executable. 
         * @param events An instance of the PrinceEvents interface that will
         * receive error/warning messages returned from Prince.
         */
        public PrinceControl(string exePath, PrinceEvents events)
            : base(exePath, events)
        {
            jobResources = new List<byte[]>();
            documents = new List<string>();
            attachments = new List<Attachment>();
        }
    
        /** Get the version string for the running Prince process.
         */
        public string GetVersion()
        {
            return mVersion;
        }
    
        /**
         * Start a Prince control process that can be used for multiple
         * consecutive document conversions.
         */
        public void Start()
        {
            if (mProcess != null)
            {
                throw new SystemException("control process has already been started");
            }
    
            string cmdLine = getBaseCommandLine();
    
            cmdLine += "--control";
    
            mProcess = StartPrince(cmdLine);
    
            StreamReader outputFromPrince = mProcess.StandardOutput;
    
            Chunk chunk = Chunk.readChunk(outputFromPrince);
    
            if (chunk.GetTag().Equals("ver"))
            {
                mVersion = chunk.GetString();
            }
            else if(chunk.GetTag().Equals("err"))
            {
                throw new IOException("error: " + chunk.GetString());
            }
            else
            {
                throw new IOException("unknown chunk: " + chunk.GetTag());
            }
    
        }
    
    
        public void Stop()
        {
            if (mProcess == null)
            {
                throw new SystemException("control process has not been started");
            }
    
            StreamWriter inputToPrince = mProcess.StandardInput;
            StreamReader outputFromPrince = mProcess.StandardOutput;
    
            mProcess = null;
    
            Chunk.writeChunk(inputToPrince.BaseStream, "end", "");
    
            inputToPrince.Close();
            outputFromPrince.Close();
        }
    
        private string GetJobJSON()
        {
            Json json = new Json();
    
            json.beginObj();
           
            json.beginObj("input");
            if (!string.IsNullOrEmpty(mInputType)) json.field("type", mInputType);
            if (!string.IsNullOrEmpty(mBaseURL)) json.field("base", mBaseURL);
            if (!string.IsNullOrEmpty(mMedia)) json.field("media", mMedia);
            json.field("javascript", mJavaScript);
            json.field("xinclude", mXInclude);
            json.field("xml-external-entities", mXmlExternalEntities);
            json.field("default-style", !mNoDefaultStyle);
            json.field("author-style", !mNoAuthorStyle);
    
            json.beginList("src");
            foreach (string doc in documents)
            {
                json.value(doc);
            }
            json.endList();
    
    
            if(!string.IsNullOrEmpty(mStyleSheets))
            {
                json.beginList("styles");
                string[] separators = new string[] { "-s \"", "\" -s \"", "\" " };
                string[] result = mStyleSheets.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in result)
                {
                    json.value(s);
                }
                json.endList();
            }
    
            if (!string.IsNullOrEmpty(mJavaScripts))
            {
                json.beginList("scripts");
                string[] separators = new string[] { "--script \"", "\" --script \"", "\" " };
                string[] result = mJavaScripts.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in result)
                {
                    json.value(s);
                }
                json.endList();
            }
            json.endObj();
            json.field("job-resource-count", jobResources.Count);
           
          
            json.beginObj("pdf");
            json.field("embed-fonts", mEmbedFonts);
            json.field("subset-fonts", mSubsetFonts);
            if (!string.IsNullOrEmpty(mPDFProfile)) json.field("pdf-profile", mPDFProfile);
            if (!string.IsNullOrEmpty(mPDFOutputIntent))
            {
                json.field("pdf-output-intent", mPDFOutputIntent);
                if (mConvertColors)
                {
                    json.field("convert-colors", mConvertColors);
                }
            }
            json.field("artificial-fonts", !mNoArtificialFonts);
            if (!string.IsNullOrEmpty(mFallbackCmykProfile)) json.field("fallback-cmyk-profile", mFallbackCmykProfile);
            json.field("force-identity-encoding", mForceIdentityEncoding);
            json.field("compress", mCompress);
            if (mEncrypt)
            {
                json.beginObj("encrypt");
                json.field("key-bits", mKeyBits);
                if (!string.IsNullOrEmpty(mUserPassword)) json.field("user-password", mUserPassword);
                if (!string.IsNullOrEmpty(mOwnerPasssword)) json.field("owner-password", mOwnerPasssword);
                json.field("disallow-print", mDisallowPrint);
                json.field("disallow-modify", mDisallowModify);
                json.field("disallow-copy", mDisallowCopy);
                json.field("disallow-annotate", mDisallowAnnotate);
                json.endObj();
            }
    
            if (!string.IsNullOrEmpty(mFileAttachments))
            {
                json.beginList("attach");
    
                string[] separators = new string[] { "--attach=\"", "\" --attach=\"", "\" " };
                string[] result = mFileAttachments.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in result)
                {
                    json.value(s);
                }
    
                foreach (Attachment att in attachments)
                {
                    json.beginObj();
                    json.field("url", att.url);
                    if (!string.IsNullOrEmpty(att.filename))
                    {
                        json.field("filename", att.filename);
    
                        if (!string.IsNullOrEmpty(att.description))
                        {
                            json.field("description", att.description);
                        }
                    }
                    json.endObj();
                }
                json.endList();
            }
            json.endObj();
            
            json.beginObj("metadata");
            if (!string.IsNullOrEmpty(mPdfTitle)) json.field("title", mPdfTitle);
            if (!string.IsNullOrEmpty(mPdfSubject)) json.field("subject", mPdfSubject);
            if (!string.IsNullOrEmpty(mPdfAuthor)) json.field("author", mPdfAuthor);
            if (!string.IsNullOrEmpty(mPdfKeywords)) json.field("keywords", mPdfKeywords);
            if (!string.IsNullOrEmpty(mPdfCreator)) json.field("creator", mPdfCreator);
            json.endObj();
            
            json.endObj();
    
            return json.toString();
        }
    
        public void AddStyleSheet(byte[] cssBytes)
        {
            jobResources.Add(cssBytes);
            AddStyleSheet("job-resource:" + (jobResources.Count - 1).ToString()); 
        }
    
        public void AddScript(byte[] scriptBytes)
        {
            jobResources.Add(scriptBytes);
            AddScript("job-resource:" + (jobResources.Count - 1).ToString());
        }
    
        public void AddFileAttachment(byte[] fileBytes, string filename = "", string description = "")
        {
            jobResources.Add(fileBytes);
            attachments.Add(new Attachment()
                            {
                                url = "job-resource:" + (jobResources.Count - 1).ToString(),
                                filename = filename,
                                description = description
                            });
        }
    
        private void AddResource(byte[] doc)
        {
            jobResources.Add(doc);
            documents.Add("job-resource:" + (jobResources.Count - 1).ToString());
        }
    
        //Reads inputDoc from a stream and writes pdfOutput to another stream.
        public new bool Convert(Stream inputDoc, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            MemoryStream mstream = new MemoryStream();
    
            CopyInputToOutput(inputDoc, mstream);
    
            byte[] input = mstream.ToArray();
    
            AddResource(input);
    
            return Convert(pdfOutput, dats);
        }
    
        //The argument inputDocs is a list of byte arrays(byte[]) representing the input documents to be converted.
        public bool Convert(List<byte[]> inputDocs, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            foreach (byte[] doc in inputDocs)
            {
                AddResource(doc);
            }
    
            return Convert(pdfOutput, dats);
        }
    
        //The argument inputDocs is an array of strings representing the filenames of the documents to be converted.
        public bool Convert(string[] inputDocs, Stream pdfOutput, List<Tuple<string, string>> dats = null)
        {
            foreach (string doc in inputDocs)
            {
                documents.Add(doc);
            }
    
            return Convert(pdfOutput, dats);
        }
    
    
        private bool Convert(Stream pdfOutput, List<Tuple<string, string>> dats)
        {
            if (mProcess == null)
            {
                throw new SystemException("control process has not been started");
            }
    
            StreamWriter inputToPrince = mProcess.StandardInput;
            StreamReader outputFromPrince = mProcess.StandardOutput;
    
      
            Chunk.writeChunk(inputToPrince.BaseStream, "job", GetJobJSON());
    
            foreach(byte[] resource in jobResources)
            {
                Chunk.writeChunk(inputToPrince.BaseStream, "dat", resource);
            }
    
    
            inputToPrince.Flush();
    
            Chunk chunk = Chunk.readChunk(outputFromPrince);
    
            if (chunk.GetTag().Equals("pdf"))
            {
                byte[] bytes = chunk.GetBytes();
                pdfOutput.Write(bytes, 0, bytes.Length);
    
                chunk = Chunk.readChunk(outputFromPrince);
            }
    
            if (chunk.GetTag().Equals("log"))
            {
                return ReadMessages(new StreamReader(new MemoryStream(chunk.GetBytes())), dats).Equals("success");
            }
            else if (chunk.GetTag().Equals("err"))
            {
                throw new IOException("error: " + chunk.GetString());
            }
            else
            {
                throw new IOException("unknown chunk: " + chunk.GetTag());
            }
        }
    
    
        public static void CopyInputToOutput(Stream input, MemoryStream output)
        {
            const int BUFSIZE = 65536;
            byte[] buf = new byte[BUFSIZE];
            int bytesRead;
    
            bytesRead = input.Read(buf, 0, BUFSIZE);
            while(bytesRead != 0)
            {
                output.Write(buf, 0, bytesRead);
                bytesRead = input.Read(buf, 0, BUFSIZE);
            }
        }
    
    
    }
    
    public class Json
    {
        private StringBuilder mStr;
        private Boolean mComma;
    
        public Json()
        {
            mStr = new StringBuilder();
            mComma = false;
        }
    
        public Json beginObj()
        {
            if (mComma) mStr.Append(',');
            mStr.Append('{');
            mComma = false;
            return this;
        }
    
        public Json beginObj(String name)
        {
            if (mComma) mStr.Append(',');
    
            mStr.Append('"');
            mStr.Append(escape(name));
            mStr.Append("\":{");
    
            mComma = false;
    
            return this;
        }
    
        public Json endObj()
        {
            mStr.Append('}');
            mComma = true;
            return this;
        }
    
        public Json beginList(string name)
        {
            if (mComma) mStr.Append(',');
    
            mStr.Append('"');
            mStr.Append(escape(name));
            mStr.Append("\":[");
    
            mComma = false;
    
            return this;
        }
    
        public Json endList()
        {
            mStr.Append(']');
            mComma = true;
            return this;
        }
    
        public Json field(string name)
        {
            if (mComma) mStr.Append(',');
    
            mStr.Append('"');
            mStr.Append(escape(name));
            mStr.Append("\":");
    
            mComma = false;
    
            return this;
        }
    
        public Json field(string name, string v)
        {
            field(name);
            value(v);
            return this;
        }
    
        public Json field(string name, Boolean v)
        {
            field(name);
            value(v);
            return this;
        }
    
        public Json field(string name, int v)
        {
            field(name);
            value(v);
            return this;
        }
    
        public Json value(string v)
        {
            if (mComma) mStr.Append(',');
    
            mStr.Append('"');
            mStr.Append(escape(v));
            mStr.Append('"');
            mComma = true;
    
            return this;
        }
    
        public Json value(int v)
        {
            if (mComma) mStr.Append(',');
            mStr.Append(v.ToString());
            mComma = true;
            return this;
        }
    
        public Json value(Boolean v)
        {
            if (mComma) mStr.Append(',');
    
            mStr.Append(v ? "true" : "false");
            mComma = true;
            return this;
        }
    
        public string toString()
        {
            return mStr.ToString();
        }
    
        private string escape(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    
    }

}