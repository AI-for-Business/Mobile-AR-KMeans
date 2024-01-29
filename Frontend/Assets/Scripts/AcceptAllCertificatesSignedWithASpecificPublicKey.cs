using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Allows to accept all certificates with specified keys.
/// Here used to accept self-signed certificates.
/// </summary>
public class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
{
    private static string PUB_KEY = "SUPER_PRIVATE_KEY";

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate certificate = new X509Certificate(certificateData);

        string pk = certificate.GetPublicKeyString();

        Debug.Log("Public key: " + pk);

        return pk.Equals(PUB_KEY);
    }
}
