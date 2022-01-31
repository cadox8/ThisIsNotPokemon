namespace pokemon {
    public class Move {
        
        public MoveBase Base { get; set; }
        public int PP { get; set;  }

        public Move(MoveBase @base)
        {
            this.Base = @base;
            this.PP = this.Base.Pp;
        }
    }
}