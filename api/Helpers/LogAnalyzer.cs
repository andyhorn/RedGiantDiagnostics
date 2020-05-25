using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities;

namespace API.Helpers
{
    public class LogAnalyzer : ILogAnalyzer
    {
        private readonly List<AnalysisResult> _results;
        private LogFile _log;

        public LogAnalyzer()
        {
            _results = new List<AnalysisResult>();
            // _log = log;
        }

        public IEnumerable<AnalysisResult> Analyze(LogFile log)
        {
            _log = log;

            // Analyze the expiration dates
            var expirationDateResults = VerifyLicenseExpirationDates();
            // _results.AddRange(expirationDateResults);
            _results.AddRange(expirationDateResults);

            // Analyze the Host IP addresses
            var networkAddressResults = VerifyHostNetworkAddresses();
            _results.AddRange(networkAddressResults);

            // Analyse the MAC addresses
            var macAddressResults = VerifyMacAddresses();
            _results.AddRange(macAddressResults);

            // Analyze the number of RLM instances running
            var rlmInstanceResults = VerifyRlmInstances();
            _results.AddRange(rlmInstanceResults);

            // Analyze 
            return _results;
        }

        private List<AnalysisResult> VerifyRlmInstances()
        {
            var results = new List<AnalysisResult>();

            if (_log.RlmInstances.Count() > 1)
            {
                var rlmInstanceWarning = new AnalysisResult
                {
                    ResultLevel = AnalysisResult.Level.Warning,
                    Message = $"There have been {_log.RlmInstances.Count()} RLM instances detected running on this server."
                };

                results.Add(rlmInstanceWarning);
            }

            return results;
        }

        private List<AnalysisResult> VerifyMacAddresses()
        {
            var results = new List<AnalysisResult>();

            foreach (var license in _log.Licenses)
            {
                if (license.HostMac != _log.HostMacList.ElementAtOrDefault(0))
                {   
                    var macWarningResult = new AnalysisResult
                    {
                        ResultLevel = AnalysisResult.Level.Error,
                        Message = $"License {license.Name} is not assigned to the host's primary MAC address."
                    };

                    results.Add(macWarningResult);
                }
            }

            return results;
        }

        private List<AnalysisResult> VerifyHostNetworkAddresses()
        {
            var results = new List<AnalysisResult>();

            foreach (var licenseFile in _log.Licenses)
            {
                if (licenseFile.HostAddress != _log.PrimaryHost)
                {
                    var hostAddressWarning = new AnalysisResult
                    {
                        ResultLevel = AnalysisResult.Level.Warning,
                        Message = $"License {licenseFile.Name} is not assigned to the host's primary IP address."
                    };

                    results.Add(hostAddressWarning);
                }
            }

            return results;
        }

        private List<AnalysisResult> VerifyLicenseExpirationDates()
        {
            var expirationAnalysisResults = new List<AnalysisResult>();

            foreach (var license in _log.Licenses)
            {
                if (AllLicensesExpired(license))
                {
                    var expirationResult = new AnalysisResult
                    {
                        ResultLevel = AnalysisResult.Level.Error,
                        Message = $"License ${license.Name} is expired."
                    };

                    expirationAnalysisResults.Add(expirationResult);
                }
                else if (SomeLicensesExpired(license))
                {
                    foreach (var product in GetExpiredProducts(license))
                    {
                        var result = new AnalysisResult
                        {
                            ResultLevel = AnalysisResult.Level.Error,
                            Message = $"The license for {product} is expired."
                        };

                        expirationAnalysisResults.Add(result);
                    }
                }
            }

            return expirationAnalysisResults;
        }

        private List<string> GetExpiredProducts(LicenseFile file)
        {
            var expiredProducts = new List<string>();

            foreach (var product in file.ProductLicenses)
            {
                if (LicenseIsExpired(product))
                {
                    expiredProducts.Add(product.ProductName);
                }
            }

            return expiredProducts;
        }

        private bool SomeLicensesExpired(LicenseFile file)
        {
            bool someExpired = false;

            foreach (var product in file.ProductLicenses)
            {
                if (LicenseIsExpired(product))
                {
                    someExpired = true;
                    break;
                }
            }

            return someExpired;
        }

        private bool AllLicensesExpired(LicenseFile file)
        {
            bool allExpired = true;

            foreach (var license in file.ProductLicenses)
            {
                if (!LicenseIsExpired(license))
                {
                    allExpired = false;
                    break;
                }
            }

            return allExpired;
        }

        private bool LicenseIsExpired(ProductLicense productLicense)
        {
            return productLicense.ExpirationDate < DateTime.Now;
        }
    }
}