namespace Parrot.client.Classes
{

    public class OwnerList
    {

        public static bool Loaded => true;

        public static int Count => 0;

        public static void EnsureLoaded() { }

        public static bool HasAccess() => false;

        public static bool TryGetName(out string name)
        {
            name = null;
            return false;
        }

        public static bool IsOwner(string userId) => false;

        public static bool IsOwnerActor(int actorNumber) => false;
    }
}
