namespace GameListDB
{
    public static class StringUtils
    {
        //
        // Summary:
        //     Checks a String against some suspected known keywords
        //
        // Parameters:
        //   value:
        //     The string to check.
        //
        // Returns:
        //     true if the value parameter match any of the ForbiddenWords
        public static bool SecurityChecks(string example)
        {
            String[] keywords = {"SELECT","ALTER","DROP","OR","AND", "EXEC", "CREATE", "\'"};
            foreach (var keyword in keywords)
            {
                if(example.Contains(keyword)){return true;};
            }
            return false;
        }
    }
}