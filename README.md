# EpicorRESTClientGenerator
Interacting with the Epicor REST API using C# requires strongly-typed classes that match the Epicor Business Objects schema. Use this utility to generate these classes from Epicor Business Objects using `NSwag.CodeGeneration.CSharp` and the Epicor REST API.

![image](https://user-images.githubusercontent.com/1199572/41175887-952f1720-6b2c-11e8-9dfd-c211f3267356.png)

## Getting Started
1. Clone this repository and build the solution, then run the `EpicorRESTGenerator.WPFGUI` project. On first run you will need to populate the settings below. The settings (except the password) are persisted to the filesystem and should load during subsequent runs.

    - Epicor URL (Full URL to the Epicor instance)
    - Namespace (Generated class namespace)
    - Base Class (Generated base class)
    - Username
    - Password
    - ERP Project (Target directory path for the generated classes)

2. After populating the required settings, click "Login" to authenticate with API.
3. After logging in, click "Get Service List" to get the list of available services from the API.
4. Select one (or multiple) services, then click "Generate Models".
5. Browse to the "ERP Project" target directory path to confirm that the classes generated correctly.

## Credits
Thanks to [Edward Welsh](https://github.com/EdWelsh) for creating this generator.
