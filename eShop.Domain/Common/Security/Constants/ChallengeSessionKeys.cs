namespace eShop.Domain.Common.Security.Constants;

public static class ChallengeSessionKeys
{
    public const string Verification = "webauthn_verification_challenge";
    public const string Attestation = "webauthn_attestation_challenge";
    public const string Assertion = "webauthn_assertion_challenge";
}