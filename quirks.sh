#!/bin/bash

# Run this script to make modifications to the AXL schema files needed to work around some WCF issues.
# You may need to modify the script to match your situation...

# This substitution will prevent SoapCore from sending a default "<customerName> xsi:nil="true" />" element
# in add/updateUser requests, which will cause the requests to fail for non-hosted CUCMs.
# If the CUCM is a Hosted Collaboration Solution Shared Architecture deployment, comment out the following:
sed -i 's/name=\"customerName\" nillable=\"true\"/name=\"customerName\" nillable=\"false\"/g' schema/AXLSoap.xsd