using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TitForTat.Services
{
    public interface IRegistrationCodeService
    {
        string GenerateCode();
        bool ValidateAndConsumeCode(string code);
        bool IsAnyCodeActive();
        IEnumerable<(string Code, DateTime Expiry)> GetActiveCodes();
    }

    public class RegistrationCodeService : IRegistrationCodeService
    {
        private readonly ConcurrentDictionary<string, DateTime> _codes = new();
        private readonly Random _random = new();

        public string GenerateCode()
        {
            CleanupExpiredCodes();

            string code;
            do
            {
                code = _random.Next(10000, 100000).ToString();
            } while (_codes.ContainsKey(code));

            var expiry = DateTime.UtcNow.AddMinutes(10);
            _codes[code] = expiry;
            return code;
        }

        public bool ValidateAndConsumeCode(string code)
        {
            CleanupExpiredCodes();

            if (string.IsNullOrWhiteSpace(code)) return false;

            if (_codes.TryGetValue(code, out var expiry))
            {
                if (expiry > DateTime.UtcNow)
                {
                    _codes.TryRemove(code, out _);
                    return true;
                }
                else
                {
                    _codes.TryRemove(code, out _);
                }
            }

            return false;
        }

        public bool IsAnyCodeActive()
        {
            CleanupExpiredCodes();
            return _codes.Any(x => x.Value > DateTime.UtcNow);
        }

        public IEnumerable<(string Code, DateTime Expiry)> GetActiveCodes()
        {
            CleanupExpiredCodes();
            return _codes.Select(x => (x.Key, x.Value)).ToList();
        }

        private void CleanupExpiredCodes()
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in _codes)
            {
                if (kvp.Value <= now)
                {
                    _codes.TryRemove(kvp.Key, out _);
                }
            }
        }
    }
}
