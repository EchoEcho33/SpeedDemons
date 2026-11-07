namespace Mechanics.Abilities
{
    public class JackalopeDig : Ability
    {
        private string abilityName = "Jackalope Dig";
        private string description = "Digs underground, making you invulnerable and ending with a shockwave!";
        
        public override int _triggerCost()
        {
            // temp cost for now
            return 1;
        }

        public override void TriggerAbility()
        {
            PlayerController controller = this.gameObject.transform.root.GetComponent<PlayerController>();
            
            
        }
        
    }
}