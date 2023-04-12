namespace Otterkit;

public static partial class Analyzer
{
    // The following methods are responsible for parsing data division clauses, 
    // each method is responsible for parsing only a single clause (Never parse two clauses with one method).
    // The IsClauseErrorCheck() method handles an 'IS' keyword potentially missing its accompanying clause.

    private static void IsClauseErrorCheck()
    {
        Error
        .Build(ErrorType.Analyzer, ConsoleColor.Red, 35, """
            Missing clause or potential clause mismatch.
            """)
        .WithSourceLine(Current(), """
            The 'IS' clause must only be followed by EXTERNAL, GLOBAL or TYPEDEF.
            """)
        .CloseError();
    }

    private static void FileEntryClauses(DataEntry fileLocal)
    {
        if (CurrentEquals("IS") && !LookaheadEquals(1, "EXTERNAL", "GLOBAL"))
        {
            IsClauseErrorCheck();
        }

        if ((CurrentEquals("IS") && LookaheadEquals(1, "EXTERNAL")) || CurrentEquals("EXTERNAL"))
        {
            ExternalClause(fileLocal);
        }

        if ((CurrentEquals("IS") && LookaheadEquals(1, "GLOBAL")) || CurrentEquals("GLOBAL"))
        {
            GlobalClause(fileLocal);
        }

        if (CurrentEquals("FORMAT"))
        {
            // TODO: This needs to be fixed later
            Expected("FORMAT");
            Choice("BIT", "CHARACTER", "NUMERIC");
            Optional("DATA");
        }

        if (CurrentEquals("BLOCK"))
        {
            Expected("BLOCK");
            Optional("CONTAINS");
            
            if (LookaheadEquals(1, "TO"))
            {
                Number();
                Expected("TO");
            }

            Number();
            Choice("CHARACTERS", "RECORDS");
        }

        if (CurrentEquals("RECORD"))
        {
            RecordClause(fileLocal);
        }

        if (CurrentEquals("LINAGE"))
        {
            LinageClause(fileLocal);
        }

        if (CurrentEquals("CODE-SET"))
        {
            CodeSetClause(fileLocal);
        }

        if (CurrentEquals("REPORT", "REPORTS"))
        {
            ReportsClause(fileLocal);
        }
    }

    private static void DataEntryClauses(DataEntry dataLocal)
    {
        if (CurrentEquals("IS") && !LookaheadEquals(1, "EXTERNAL", "GLOBAL", "TYPEDEF"))
        {
            IsClauseErrorCheck();
        }

        if ((CurrentEquals("IS") && LookaheadEquals(1, "EXTERNAL")) || CurrentEquals("EXTERNAL"))
        {
            ExternalClause(dataLocal);
        }

        if ((CurrentEquals("IS") && LookaheadEquals(1, "GLOBAL")) || CurrentEquals("GLOBAL"))
        {
            GlobalClause(dataLocal);
        }

        if ((CurrentEquals("IS") && LookaheadEquals(1, "TYPEDEF")) || CurrentEquals("TYPEDEF"))
        {
            TypedefClause(dataLocal);
        }

        if (CurrentEquals("REDEFINES"))
        {
            RedefinesClause(dataLocal);
        }

        if (CurrentEquals("ALIGNED"))
        {
            AlignedClause(dataLocal);
        }

        if (CurrentEquals("ANY") && LookaheadEquals(1, "LENGTH"))
        {
            AnyLengthClause(dataLocal);
        }

        if (CurrentEquals("BASED"))
        {
            BasedClause(dataLocal);
        }

        if (CurrentEquals("BLANK"))
        {
            BlankWhenClause(dataLocal);
        }

        if (CurrentEquals("CONSTANT") && LookaheadEquals(1, "RECORD"))
        {
            ConstantRecordClause(dataLocal);
        }

        if (CurrentEquals("DYNAMIC"))
        {
            DynamicClause(dataLocal);
        }

        if (CurrentEquals("GROUP-USAGE"))
        {
            GroupUsageClause(dataLocal);
        }

        if (CurrentEquals("JUSTIFIED", "JUST"))
        {
            JustifiedClause(dataLocal);
        }

        if (CurrentEquals("SYNCHRONIZED", "SYNC"))
        {
            SynchronizedClause(dataLocal);
        }

        if (CurrentEquals("PROPERTY"))
        {
            PropertyClause(dataLocal);
        }

        if (CurrentEquals("SAME"))
        {
            SameAsClause(dataLocal);
        }

        if (CurrentEquals("TYPE"))
        {
            TypeClause(dataLocal);
        }

        if (CurrentEquals("OCCURS"))
        {
            OccursClause(dataLocal);
        }

        if (CurrentEquals("PIC", "PICTURE"))
        {
            PictureClause(dataLocal);
        }

        if (CurrentEquals("VALUE"))
        {
            ValueClause(dataLocal);
        }

        if (CurrentEquals("USAGE"))
        {
            UsageClause(dataLocal);
        }
    }

    private static void ScreenEntryClauses(DataEntry screenLocal)
    {
        if ((CurrentEquals("IS") && LookaheadEquals(1, "GLOBAL")) || CurrentEquals("GLOBAL"))
        {
            GlobalClause(screenLocal);
        }

        if (CurrentEquals("LINE"))
        {
            LineClause(screenLocal);
        }

        if (CurrentEquals("COLUMN", "COL"))
        {
            ColumnClause(screenLocal);
        }

        if (CurrentEquals("PICTURE", "PIC"))
        {
            PictureClause(screenLocal);
        }

        if (CurrentEquals("BLANK") && LookaheadEquals(1, "SCREEN", "LINE"))
        {
            Expected("BLANK");
            Choice("SCREEN", "LINE");
        }

        if (CurrentEquals("BLANK") && !LookaheadEquals(1, "SCREEN", "LINE"))
        {
            BlankWhenClause(screenLocal);
        }

        if (CurrentEquals("JUSTIFIED", "JUST"))
        {
            JustifiedClause(screenLocal);
        }

        if (CurrentEquals("SIGN"))
        {
            SignClause(screenLocal);
        }

        if (CurrentEquals("FULL"))
        {
            Expected("FULL");
        }

        if (CurrentEquals("AUTO"))
        {
            Expected("AUTO");
        }

        if (CurrentEquals("SECURE"))
        {
            Expected("SECURE");
        }

        if (CurrentEquals("REQUIRED"))
        {
            Expected("REQUIRED");
        }

        if (CurrentEquals("BELL"))
        {
            Expected("BELL");
        }

        if (CurrentEquals("HIGHLIGHT", "LOWLIGHT"))
        {
            Choice("HIGHLIGHT", "LOWLIGHT");
        }

        if (CurrentEquals("REVERSE-VIDEO"))
        {
            Expected("REVERSE-VIDEO");
        }

        if (CurrentEquals("UNDERLINE"))
        {
            Expected("UNDERLINE");
        }

        if (CurrentEquals("FOREGROUND-COLOR"))
        {
            Expected("FOREGROUND-COLOR");
            Optional("IS");
            if (CurrentEquals(TokenType.Identifier))
            {
                Identifier();
            }
            else
            {
                Number();
            }
        }

        if (CurrentEquals("BACKGROUND-COLOR"))
        {
            Expected("BACKGROUND-COLOR");
            Optional("IS");
            if (CurrentEquals(TokenType.Identifier))
            {
                Identifier();
            }
            else
            {
                Number();
            }
        }

        if (CurrentEquals("OCCURS"))
        {
            Expected("OCCURS");
            Number();

            Optional("TIMES");
        }

        if (CurrentEquals("USAGE"))
        {
            Expected("USAGE");
            Optional("IS");

            Choice("DISPLAY", "NATIONAL");
        }

        ScreenValueClause(screenLocal);
    }

    private static void LinageClause(DataEntry fileLocal)
    {
        Expected("LINAGE");
        Optional("IS");

        IdentifierOrLiteral(TokenType.Numeric);
        Optional("LINES");

        if (CurrentEquals("WITH", "FOOTING"))
        {
            Optional("WITH");
            Expected("FOOTING");
            IdentifierOrLiteral(TokenType.Numeric);
        }

        if (CurrentEquals("LINES", "AT", "TOP"))
        {
            Optional("LINES");
            Optional("AT");
            Expected("TOP");
            IdentifierOrLiteral(TokenType.Numeric);
        }

        if (CurrentEquals("LINES", "AT", "BOTTOM"))
        {
            Optional("LINES");
            Optional("AT");
            Expected("BOTTOM");
            IdentifierOrLiteral(TokenType.Numeric);
        }
    }

    private static void RecordClause(DataEntry fileLocal)
    {
        Expected("RECORD");

        if (CurrentEquals("IS", "VARYING"))
        {
            Optional("IS");
            Expected("VARYING");
            Optional("IN");
            Optional("SIZE");

            if (CurrentEquals("FROM") || CurrentEquals(TokenType.Numeric))
            {
                Optional("FROM");
                Number();
            }

            if (CurrentEquals("TO"))
            {
                Optional("TO");
                Number();
            }
            
            if (CurrentEquals("BYTES", "CHARACTERS"))
            {
                Expected(Current().Value);
            }

            if (CurrentEquals("DEPENDING"))
            {
                Expected("DEPENDING");
                Optional("ON");
                Identifier();
            }

            return;
        }

        // If the record is not varying in size
        Optional("CONTAINS");
        
        if (!LookaheadEquals(1, "TO"))
        {
            Number();

            if (CurrentEquals("BYTES", "CHARACTERS"))
            {
                Expected(Current().Value);
            }

            return;
        }

        // If the record is fixed-or-variable
        Number();

        Expected("TO");

        Number();

        if (CurrentEquals("BYTES", "CHARACTERS"))
        {
            Expected(Current().Value);
        }
    }

    private static void ReportsClause(DataEntry fileLocal)
    {
        Choice("REPORT", "REPORTS");

        if (LookaheadEquals(-1, "REPORT"))
        {
            Optional("IS");
        }
        else
        {
            Optional("ARE");
        }

        Identifier();

        while (CurrentEquals(TokenType.Identifier))
        {
            Identifier();
        }
    }

    private static void CodeSetClause(DataEntry fileLocal)
    {
        Expected("CODE-SET");

        if (CurrentEquals("FOR", "ALPHANUMERIC", "NATIONAL"))
        {
            ForAlphanumericForNational();
            return;
        }

        Optional("IS");
        Identifier();

        if (CurrentEquals(TokenType.Identifier))
        {
            Identifier();
        } 
    }

    private static void ScreenValueClause(DataEntry screenLocal)
    {
        if (CurrentEquals("FROM"))
        {
            Expected("FROM");
            IdentifierOrLiteral();

            return;
        }

        if (CurrentEquals("TO"))
        {
            Expected("TO");
            Identifier();

            return;
        }

        if (CurrentEquals("USING"))
        {
            Expected("USING");
            Identifier();

            return;
        }

        if (CurrentEquals("VALUE"))
        {
            Expected("VALUE");
            Optional("IS");

            ParseLiteral(true, true);
        }
    }

    private static void LineClause(DataEntry screenLocal)
    {
        Expected("LINE");
        Optional("NUMBER");
        Optional("IS");

        if (CurrentEquals("PLUS", "+", "MINUS", "-"))
        {
            Expected(Current().Value);
        }

        if (CurrentEquals(TokenType.Identifier))
        {
            Identifier();
        }
        else
        {
            Number();
        }
    }

    private static void ColumnClause(DataEntry screenLocal)
    {
        Choice("COLUMN", "COL");
        Optional("NUMBER");
        Optional("IS");

        if (CurrentEquals("PLUS", "+", "MINUS", "-"))
        {
            Expected(Current().Value);
        }

        if (CurrentEquals(TokenType.Identifier))
        {
            Identifier();
        }
        else
        {
            Number();
        }
    }

    private static void SignClause(DataEntry entryLocal)
    {
        Expected("SIGN");
        Optional("IS");

        Choice("LEADING", "TRAILING");

        if (CurrentEquals("SEPARATE"))
        {
            Expected("SEPARATE");
            Optional("CHARACTER");
        }
    }

    private static void ExternalClause(DataEntry entryLocal)
    {
        Optional("IS");
        Expected("EXTERNAL");
        if (CurrentEquals("AS"))
        {
            Expected("AS");
            entryLocal.IsExternal = true;
            entryLocal.ExternalizedName = Current().Value;

            String();
        }

        if (!CurrentEquals("AS"))
        {
            entryLocal.IsExternal = true;
            entryLocal.ExternalizedName = Current().Value;
        }
    }

    private static void GlobalClause(DataEntry entryLocal)
    {
        Optional("IS");
        Expected("GLOBAL");
        entryLocal.IsGlobal = true;
    }

    private static void TypedefClause(DataEntry entryLocal)
    {
        Optional("IS");
        Expected("TYPEDEF");
        entryLocal.IsTypedef = true;

        if (CurrentEquals("STRONG")) Expected("STRONG");
    }

    private static void RedefinesClause(DataEntry entryLocal)
    {
        Expected("REDEFINES");
        Identifier();
        entryLocal.IsRedefines = true;
    }

    private static void AlignedClause(DataEntry entryLocal)
    {
        Expected("ALIGNED");
    }

    private static void AnyLengthClause(DataEntry entryLocal)
    {
        Expected("ANY");
        Expected("LENGTH");
        entryLocal.IsAnyLength = true;
    }

    private static void BasedClause(DataEntry entryLocal)
    {
        Expected("BASED");
        entryLocal.IsBased = true;
    }

    private static void BlankWhenClause(DataEntry entryLocal)
    {
        Expected("BLANK");
        Optional("WHEN");
        Expected("ZERO");
        entryLocal.IsBlank = true;
    }

    private static void ConstantRecordClause(DataEntry entryLocal)
    {
        Expected("CONSTANT");
        Expected("RECORD");
        entryLocal.IsConstantRecord = true;
    }

    private static void DynamicClause(DataEntry entryLocal)
    {
        Expected("DYNAMIC");
        Optional("LENGTH");
        entryLocal.IsDynamicLength = true;

        if (CurrentEquals(TokenType.Identifier)) Identifier();

        if (CurrentEquals("LIMIT"))
        {
            Expected("LIMIT");
            Optional("IS");
            Number();
        }
    }

    private static void GroupUsageClause(DataEntry entryLocal)
    {
        Expected("GROUP-USAGE");
        Optional("IS");
        Choice("BIT", "NATIONAL");
    }

    private static void JustifiedClause(DataEntry entryLocal)
    {
        Choice("JUSTIFIED", "JUST");
        Optional("RIGHT");
    }

    private static void SynchronizedClause(DataEntry entryLocal)
    {
        Choice("SYNCHRONIZED", "SYNC");
        if (CurrentEquals("LEFT")) Expected("LEFT");

        else if (CurrentEquals("RIGHT")) Expected("RIGHT");
    }

    private static void PropertyClause(DataEntry entryLocal)
    {
        Expected("PROPERTY");
        entryLocal.IsProperty = true;
        if (CurrentEquals("WITH", "NO"))
        {
            Optional("WITH");
            Expected("NO");
            Choice("GET", "SET");
        }

        if (CurrentEquals("IS", "FINAL"))
        {
            Optional("IS");
            Expected("FINAL");
        }
    }

    private static void SameAsClause(DataEntry entryLocal)
    {
        Expected("SAME");
        Expected("AS");
        Identifier();
    }

    private static void TypeClause(DataEntry entryLocal)
    {
        Expected("TYPE");
        Identifier();
    }

    private static void OccursClause(DataEntry entryLocal)
    {
        Expected("OCCURS");

        if (CurrentEquals("DYNAMIC"))
        {
            Expected("DYNAMIC");

            if (CurrentEquals("CAPACITY"))
            {
                Expected("CAPACITY");
                Optional("IN");
                Identifier();
            }

            if (CurrentEquals("FROM"))
            {
                Expected("FROM");
                Number();
            }

            if (CurrentEquals("TO"))
            {
                Expected("TO");
                Number();
            }

            if (CurrentEquals("INITIALIZED"))
            {
                Expected("INITIALIZED");
            }

            AscendingDescendingKey();

            if (CurrentEquals("INDEXED"))
            {
                IndexedBy();
            }

            return;
        }

        if (LookaheadEquals(1, "TO"))
        {
            Number();
            Expected("TO");

            Number();
            Optional("TIMES");

            Expected("DEPENDING");
            Optional("ON");

            Identifier();

            AscendingDescendingKey();

            if (CurrentEquals("INDEXED"))
            {
                IndexedBy();
            }

            return;
        }

        Number();
        Optional("TIMES");

        AscendingDescendingKey();

        if (CurrentEquals("INDEXED"))
        {
            IndexedBy();
        }

        static void IndexedBy()
        {
            Expected("INDEXED");
            Optional("BY");

            Identifier();
            while (CurrentEquals(TokenType.Identifier))
            {
                Identifier();
            }
        }
    }

    private static void PictureClause(DataEntry entryLocal)
    {
        Choice("PIC", "PICTURE");
        Optional("IS");

        var picture = Current();

        var isValidPicture = PictureString(picture.Value, out var size);

        entryLocal.PictureString = picture.Value;

        entryLocal.PictureLength = size;

        entryLocal.HasPicture = true;

        Continue();
    }

    private static void ValueClause(DataEntry entryLocal)
    {
        Expected("VALUE");

        if (!CurrentEquals(TokenType.String, TokenType.Numeric))
        {
            Error
            .Build(ErrorType.Analyzer, ConsoleColor.Red, 2, """
                Unexpected token.
                """)
            .WithSourceLine(Current(), """
                Expected a string or numeric literal.
                """)
            .CloseError();
        }

        if (CurrentEquals(TokenType.String))
        {
            entryLocal.DefaultValue = Current().Value;
            String();
        }

        if (CurrentEquals(TokenType.Numeric))
        {
            entryLocal.DefaultValue = Current().Value;
            Number();
        }
    }

    private static void UsageClause(DataEntry entryLocal)
    {
        Expected("USAGE");
        Optional("IS");
        switch (Current().Value)
        {
            case "BINARY":
                Expected("BINARY");
                entryLocal.UsageType = UsageType.Binary;
                break;

            case "BINARY-CHAR":
            case "BINARY-SHORT":
            case "BINARY-LONG":
            case "BINARY-DOUBLE":
                Expected(Current().Value);
                if (CurrentEquals("SIGNED"))
                {
                    Expected("SIGNED");
                }
                else if (CurrentEquals("UNSIGNED"))
                {
                    Expected("UNSIGNED");
                }
                break;

            case "BIT":
                Expected("BIT");
                entryLocal.UsageType = UsageType.Bit;
                break;

            case "COMP":
            case "COMPUTATIONAL":
                Expected(Current().Value);
                entryLocal.UsageType = UsageType.Computational;
                break;

            case "DISPLAY":
                Expected("DISPLAY");
                entryLocal.UsageType = UsageType.Display;
                break;

            case "FLOAT-BINARY-32":
                Expected("FLOAT-BINARY-32");
                Choice("HIGH-ORDER-LEFT", "HIGH-ORDER-RIGHT");
                break;

            case "FLOAT-BINARY-64":
                Expected("FLOAT-BINARY-64");
                Choice("HIGH-ORDER-LEFT", "HIGH-ORDER-RIGHT");
                break;

            case "FLOAT-BINARY-128":
                Expected("FLOAT-BINARY-128");
                Choice("HIGH-ORDER-LEFT", "HIGH-ORDER-RIGHT");
                break;

            case "FLOAT-DECIMAL-16":
                Expected("FLOAT-DECIMAL-16");
                EncodingEndianness();
                break;

            case "FLOAT-DECIMAL-32":
                Expected("FLOAT-DECIMAL-32");
                EncodingEndianness();
                break;

            case "FLOAT-EXTENDED":
                Expected("FLOAT-EXTENDED");
                break;

            case "FLOAT-LONG":
                Expected("FLOAT-LONG");
                break;

            case "FLOAT-SHORT":
                Expected("FLOAT-SHORT");
                break;

            case "INDEX":
                Expected("INDEX");
                entryLocal.UsageType = UsageType.Index;
                break;

            case "MESSAGE-TAG":
                Expected("MESSAGE-TAG");
                entryLocal.UsageType = UsageType.MessageTag;
                break;

            case "NATIONAL":
                Expected("NATIONAL");
                entryLocal.UsageType = UsageType.National;
                break;

            case "OBJECT":
                Expected("OBJECT");
                Expected("REFERENCE");
                // var isFactory = false;
                // var isStronglyTyped = false;

                // Need implement identifier resolution first
                // To parse the rest of this using clause correctly
                entryLocal.UsageType = UsageType.ObjectReference;
                if (CurrentEquals("Factory"))
                {
                    Expected("FACTORY");
                    Optional("OF");
                    // isFactory = true;
                }

                if (CurrentEquals("ACTIVE-CLASS"))
                {
                    Expected("ACTIVE-CLASS");
                    break;
                }

                Continue();

                if (CurrentEquals("ONLY"))
                {
                    Expected("ONLY");
                    // isStronglyTyped = true
                }

                break;

            case "PACKED-DECIMAL":
                Expected("PACKED-DECIMAL");
                if (CurrentEquals("WITH", "NO"))
                {
                    Optional("WITH");
                    Expected("NO");
                    Expected("SIGN");
                }
                break;

            case "POINTER":
                Expected("POINTER");
                if (CurrentEquals("TO") || CurrentEquals(TokenType.Identifier))
                {
                    Optional("TO");
                    entryLocal.UsageType = UsageType.DataPointer;
                    entryLocal.UsageContext = Current().Value;
                    Identifier();
                }
                else
                {
                    entryLocal.UsageType = UsageType.DataPointer;
                }
                break;

            case "FUNCTION-POINTER":
                Expected("FUNCTION-POINTER");
                Optional("TO");
                entryLocal.UsageType = UsageType.FunctionPointer;
                entryLocal.UsageContext = Current().Value;
                Identifier();
                break;

            case "PROGRAM-POINTER":
                Expected("PROGRAM-POINTER");
                if (CurrentEquals("TO") || CurrentEquals(TokenType.Identifier))
                {
                    Optional("TO");
                    entryLocal.UsageType = UsageType.ProgramPointer;
                    entryLocal.UsageContext = Current().Value;
                    Identifier();
                }
                else
                {
                    entryLocal.UsageType = UsageType.ProgramPointer;
                }
                break;

            default:
                Error
                .Build(ErrorType.Analyzer, ConsoleColor.Red, 50, """
                    Unrecognized USAGE clause.
                    """)
                .WithSourceLine(Lookahead(-1))
                .WithNote("""
                    This could be due to an unsupported third-party extension.
                    """)
                .CloseError();

                AnchorPoint(TokenContext.IsClause);
                break;
        }
    }
}