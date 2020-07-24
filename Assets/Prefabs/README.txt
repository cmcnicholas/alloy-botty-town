the ItemPrefabs prefab in this directory contains all the "Assets" we want to use.
they are named specifically and referenced in code to associate dodi's to models.

make sure you.

1. create an empty parent named correctly (the key will be used to match to a dodi)
2. add the model as a child of the parent, call it Asset
3. add the AssetController script to the parent and link the Asset property to the child Asset
4. go find any switch statements that need to incorporate the new asset using your name as the key
5. set the Asset to "not receive shadows"
6. set the Asset reflection probes set to "simple"
7. add a "Mesh Collider" to the Asset (otherwise you won't be able to highlight it and you can run through it)