using System.Collections.Generic;

public interface IObjectPoolBehaviour
{
  IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos();
}
