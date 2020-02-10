using WingsOn.Domain;

namespace WingsOn.Servces
{
	public interface IPersonService
	{
		Person Get(int id);

		void Save(Person element);
	}
}
