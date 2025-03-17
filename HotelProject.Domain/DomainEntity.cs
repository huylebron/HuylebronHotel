using System . ComponentModel . DataAnnotations ;

namespace HotelProject . Domain ;

public abstract class DomainEntity < TKey >
{
    [ Key ] public TKey Id { get ; set ; }
}