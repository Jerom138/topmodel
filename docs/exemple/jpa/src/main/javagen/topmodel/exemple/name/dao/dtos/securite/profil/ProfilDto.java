////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.exemple.name.dao.dtos.securite.profil;

import java.io.Serializable;
import java.util.List;
import java.util.stream.Collectors;

import javax.annotation.Generated;

import topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto;
import topmodel.exemple.name.dao.entities.securite.profil.Profil;
import topmodel.exemple.name.dao.entities.securite.profil.TypeProfil;

/**
 * Objet métier non persisté représentant Profil.
 */
@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public class ProfilDto implements Serializable {
	/** Serial ID */
	private static final long serialVersionUID = 1L;

	/**
	 * Id technique.
	 * Alias of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#getId() Profil#getId()} 
	 */
	private long id;

	/**
	 * Type de profil.
	 * Alias of {@link topmodel.exemple.name.dao.entities.securite.profil.Profil#getTypeProfilCode() Profil#getTypeProfilCode()} 
	 */
	private TypeProfil.Values typeProfilCode;

	/**
	 * Liste paginée des utilisateurs de ce profil.
	 */
	private List<UtilisateurDto> utilisateurs;

	/**
	 * No arg constructor.
	 */
	public ProfilDto() {
	}

	/**
	 * Copy constructor.
	 * @param profilDto to copy
	 */
	public ProfilDto(ProfilDto profilDto) {
		if(profilDto == null) {
			return;
		}

		this.id = profilDto.getId();
		this.typeProfilCode = profilDto.getTypeProfilCode();

		this.utilisateurs = profilDto.getUtilisateurs().stream().collect(Collectors.toList());
	}

	/**
	 * All arg constructor.
	 * @param id Id technique
	 * @param typeProfilCode Type de profil
	 * @param utilisateurs Liste paginée des utilisateurs de ce profil
	 */
	public ProfilDto(long id, TypeProfil.Values typeProfilCode, List<UtilisateurDto> utilisateurs) {
		this.id = id;
		this.typeProfilCode = typeProfilCode;
		this.utilisateurs = utilisateurs;
	}

	/**
	 * Crée une nouvelle instance de 'ProfilDto'.
	 * @param profil Instance de 'Profil'.
	 *
	 * @return Une nouvelle instance de 'ProfilDto'.
	 */
	public ProfilDto(Profil profil) {
		this.from(profil);
	}

	/**
	 * Map les champs des classes passées en paramètre dans l'instance courante.
	 * @param profil Instance de 'Profil'.
	 */
	protected void from(Profil profil) {
		if(profil != null) {
			this.id = profil.getId();

			if(profil.getTypeProfil() != null) {
				this.typeProfilCode = profil.getTypeProfil().getCode();
			}

		}

	}

	/**
	 * Getter for id.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.profil.ProfilDto#id id}.
	 */
	public long getId() {
		return this.id;
	}

	/**
	 * Getter for typeProfilCode.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.profil.ProfilDto#typeProfilCode typeProfilCode}.
	 */
	public TypeProfil.Values getTypeProfilCode() {
		return this.typeProfilCode;
	}

	/**
	 * Getter for utilisateurs.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.profil.ProfilDto#utilisateurs utilisateurs}.
	 */
	public List<UtilisateurDto> getUtilisateurs() {
		return this.utilisateurs;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.profil.ProfilDto#id id}.
	 * @param id value to set
	 */
	public void setId(long id) {
		this.id = id;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.profil.ProfilDto#typeProfilCode typeProfilCode}.
	 * @param typeProfilCode value to set
	 */
	public void setTypeProfilCode(TypeProfil.Values typeProfilCode) {
		this.typeProfilCode = typeProfilCode;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.profil.ProfilDto#utilisateurs utilisateurs}.
	 * @param utilisateurs value to set
	 */
	public void setUtilisateurs(List<UtilisateurDto> utilisateurs) {
		this.utilisateurs = utilisateurs;
	}

	/**
	 * Mappe 'ProfilDto' vers 'Profil'.
	 * @param source Instance de 'ProfilDto'.
	 * @param dest Instance pré-existante de 'Profil'. Une nouvelle instance sera créée si non spécifié.
	 *
	 * @return Une instance de 'Profil'.
	 */
	public Profil toProfil(Profil dest) {
		dest = dest == null ? new Profil() : dest;

		dest.setId(this.getId());

		return dest;
	}
}
