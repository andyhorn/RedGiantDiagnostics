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

            // Analyze the ISV server assignments
            var isvServerAssignmentResults = VerifyIsvServers();
            _results.AddRange(isvServerAssignmentResults);

            // Analyze the existence of Red Giant licenses
            var licenseCountResult = VerifyLicenseCount();
            if (licenseCountResult != null)
                _results.Add(licenseCountResult);

            var licenseUseResults = VerifyLicenseUse();
            _results.AddRange(licenseUseResults);

            return _results;
        }

        private List<AnalysisResult> VerifyLicenseUse()
        {
            var results = new List<AnalysisResult>();

            foreach (var isv in _log.IsvStatistics)
            {
                foreach (var pool in isv.LicensePools)
                {
                    if (pool.Available == 0)
                    {
                        var outOfSeats = new AnalysisResult()
                        {
                            Message = $"{pool.Product} is out of available seats.",
                            ResultLevel = AnalysisResult.Level.Error,
                            ResultType = AnalysisResult.Type.AllLicensesInUse
                        };

                        results.Add(outOfSeats);
                    }
                    else if (pool.Available < 2)
                    {
                        var nearlyOutOfSeats = new AnalysisResult()
                        {
                            Message = $"{pool.Product} is nearly out of available seats.",
                            ResultLevel = AnalysisResult.Level.Warning,
                            ResultType = AnalysisResult.Type.NearlyAllLicensesInUse
                        };

                        results.Add(nearlyOutOfSeats);
                    }
                }
            }

            return results;
        }

        private AnalysisResult VerifyLicenseCount()
        {
            var redGiantLicenses = _log.Licenses.Where(license =>
            {
                var name = license.Name;
                name = name.Trim().ToLower().Replace(" ", "");
                return name.Contains("redgiant");
            });

            if (redGiantLicenses.Count() > 0)
            {
                return null;
            }

            redGiantLicenses = _log.Licenses.Where(license =>
            {
                var name = license.Name;
                name = name.Trim().ToLower().Replace(" ", "");
                return name.Contains("red") || name.Contains("giant");
            });

            if (redGiantLicenses.Count() == 0)
            {
                return new AnalysisResult
                {
                    Message = "No Red Giant licenses found.",
                    ResultLevel = AnalysisResult.Level.Error,
                    ResultType = AnalysisResult.Type.NoLicensesFound
                };
            }
            else
            {
                return new AnalysisResult
                {
                    Message = "Make sure there is a Red Giant license present.",
                    ResultLevel = AnalysisResult.Level.Suggestion,
                    ResultType = AnalysisResult.Type.NoLicensesFound
                };
            }
        }

        private List<AnalysisResult> VerifyIsvServers()
        {
            var results = new List<AnalysisResult>();

            foreach (var license in _log.Licenses)
            {
                if (UsesAssignedIsvPort(license))
                {
                    var portBeingUsed = GetAssignedPortForIsv(license.IsvName);
                    if (license.IsvPort != portBeingUsed)
                    {
                        var isvPortError = new AnalysisResult
                        {
                            ResultLevel = AnalysisResult.Level.Warning,
                            ResultType = AnalysisResult.Type.MismatchedIsvPort,
                            Message = $"License {license.Name} is assigned to ISV port {license.IsvPort}, " +
                            $"but the server is using {portBeingUsed}"
                        };

                        results.Add(isvPortError);
                    }
                }
            }

            return results;
        }

        private string GetAssignedPortForIsv(string isvName)
        {
            var isv = _log.RlmStatistics.Servers.FirstOrDefault(x
                => x.Name.ToLower() == isvName.ToLower());

            if (isv == null)
            {
                return string.Empty;
            }

            return isv.Port.ToString();
        }

        private bool UsesAssignedIsvPort(LicenseFile license)
        {
            return !string.IsNullOrEmpty(license.IsvPort);
        }

        private List<AnalysisResult> VerifyRlmInstances()
        {
            var results = new List<AnalysisResult>();

            if (_log.RlmInstances.Count() > 1)
            {
                var rlmInstanceWarning = new AnalysisResult
                {
                    ResultLevel = AnalysisResult.Level.Warning,
                    ResultType = AnalysisResult.Type.MultipleRlmInstances,
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
                        ResultType = AnalysisResult.Type.MismatchedMac,
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
                if (licenseFile.HostAddress != _log.PrimaryHostAddress)
                {
                    var hostAddressWarning = new AnalysisResult
                    {
                        ResultLevel = AnalysisResult.Level.Warning,
                        ResultType = AnalysisResult.Type.MismatchedIp,
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
                        ResultType = AnalysisResult.Type.ExpiredLicense,
                        Message = $"License {license.Name} is expired."
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
                            ResultType = AnalysisResult.Type.ExpiredLicense,
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