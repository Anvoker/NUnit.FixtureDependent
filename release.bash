#!/usr/bin/env bash
shopt -s extglob

parse_version_string_to_array() {
    local version_string="$1"
    local -n A832FD_version_array="$2"
    local suffix

    # Get the version string without suffix
    local version_string_no_suffix="$version_string"
    version_string_no_suffix="${version_string_no_suffix%%-*}"
    version_string_no_suffix="${version_string_no_suffix%%+*}"

    # Parse MAJOR.MINOR.PATCH integers into array.
    IFS=$'.' read -ra A832FD_version_array <<< "$version_string_no_suffix"

    # Check for the existence of a suffix starting with dash.
    suffix="${version_string#*-}"
    if [[ "$suffix" != "$version_string" ]] ; then
        if [[ -z $suffix ]] ; then
            return 1
        fi
        A832FD_version_array+=("$suffix")
        return 0
    fi

    # Check for the existence of a suffix starting with plus.
    suffix="${version_string#*+}"
    if [[ "$suffix" != "$version_string" ]] ; then
        if [[ -z $suffix ]] ; then
            return 1
        fi
        A832FD_version_array+=("$suffix")
        return 0
    fi
}

compare_version_strings() {
    local -r version_a_string="$1"
    local -r version_b_string="$2"
    local -a version_a_array
    local -a version_b_array

    parse_version_string_to_array "$version_a_string" version_a_array
    parse_version_string_to_array "$version_b_string" version_b_array

    # Compare MAJOR.MINOR.PATCH integers
    for i in {0..2} ; do
        if ((version_a_array[i] > version_b_array[i])) ; then
            return 1
        elif ((version_a_array[i] < version_b_array[i])) ; then
            return 2
        fi
    done

    local -r version_a_suffix=${version_a_array[3]}
    local -r version_b_suffix=${version_b_array[3]}

    # Compare suffixes.
    # Version without suffix is considered newer.
    # If both have a suffix then we compare lexicographically.
    if [[ -n "$version_a_suffix" ]] ; then
        if [[ -n "$version_b_suffix" ]] ; then
            if [[ "$version_a_suffix" > "$version_b_suffix" ]] ; then
                return 1
            elif [[ "$version_a_suffix" < "$version_b_suffix" ]] ; then
                return 2
            else
                return 0
            fi
        else
            return 2
        fi
    else
        if [[ -n "$version_b_suffix" ]] ; then
            return 1
        else
            return 0
        fi
    fi
}

get_version_from_csproj() {
    local -r csproj_path="$1"
    local -r version_xml_node='//Project/PropertyGroup/Version'
    xmlstarlet sel --text --template --value-of "$version_xml_node" "$csproj_path"
}

get_release_notes_from_csproj() {
    local -r csproj_path="$1"
    local -r version_xml_node='//Project/PropertyGroup/PackageReleaseNotes'
    xmlstarlet sel --text --template --value-of "$version_xml_node" "$csproj_path"
}

get_package_id_from_csproj() {
    local -r csproj_path="$1"
    local -r version_xml_node='//Project/PropertyGroup/PackageId'
    xmlstarlet sel --text --template --value-of "$version_xml_node" "$csproj_path"
}

get_nupkg_filename() {
    local -r csproj_path="$1"
    local package_id
    local version

    package_id="$(get_package_id_from_csproj "$csproj_path")"; readonly pacakge_id
       version="$(get_version_from_csproj "$csproj_path")"   ; readonly version
    
    echo "${package_id}.${version}.nupkg"
}

get_new_release_notes_from_user() {
    local notes_new
    IFS=$'\n\r' read -r -e -p "${bold}Enter new package release notes (leave empty to keep current):${normal} " notes_new
    history -s "$notes_new"
    echo "$notes_new"
}

get_version_new_from_user() {
    local -r version_current="$1"
    local -i is_version_format_valid=1
    local -i is_version_newer=1
    local version_new

    # Loop input request until the user gets it right.
    while ((is_version_format_valid != 0 || is_version_newer != 0)) ; do
        read -r -e -p "${bold}Enter new version (leave empty to keep current):${normal} " version_new
        history -s "$version_new"

        if [[ -z $version_new ]] ; then
            return 0
        fi

        # Validate new version format.
        validate_version_format "$version_new"
        is_version_format_valid=$?

        if ((is_version_format_valid != 0)) ; then
            echo "Version \"$version_new\" is an invalid version." >&2
            echo "Version format has to be: \"x.y.z[(-|+)suffix]\"." >&2
            echo "Where x, y, z are all integers and suffix is optional and can only contain alphanumeric characters and dashes." >&2
            continue
        fi

        # Check that the new version is actually newer.
        compare_version_strings "$version_new" "$version_current";
        if (($? == 1)) ; then
            is_version_newer=0
        else
            is_version_newer=1
        fi

        if ((is_version_newer != 0)) ; then
            echo "The new version has to be greater than the current version. Please try again." >&2
            continue
        fi
    done
    echo "$version_new"
}

set_version_to_csproj() {
    local -r version_new="$1"
    local -r csproj_path="$2"
    local -r version_xml_node='//Project/PropertyGroup/Version'
    xmlstarlet ed --pf --omit-decl --inplace --update "$version_xml_node" --value "$version_new" "$csproj_path"
    return "$?"
}

set_release_notes_to_csproj() {
    local -r release_notes="$1"
    local -r csproj_path="$2"
    local -r version_xml_node='//Project/PropertyGroup/PackageReleaseNotes'
    xmlstarlet ed --pf --omit-decl --inplace --update "$version_xml_node" --value "$release_notes" "$csproj_path"
    return "$?"
}

validate_version_format() {
    local -r version="$1"
    local -a version_array

    # Check number of dots == 2
    local -r dots="${version//[^.]}"
    local -ri ndots="${#dots}"

    if (( ndots != 2 )) ; then
        return 1
    fi

    parse_version_string_to_array "$version" version_array

    # Guard against empty suffix.
    if (($? == 1)) ; then
        return 1
    fi

    # Make sure the array has 3 or 4 elements
    local -ri length="${#version_array[@]}"
    if (( length > 4 , length < 3 )) ; then
        return 0
    fi

    # Check that the MAJOR.MINOR.PATCH integers are actually integers.
    for i in 0 1 2 ; do
        local piece="${version_array[i]}"
        if [[ "${piece:0:1}" = '0' ]] ; then
            if [[ "$piece" = '0' ]] ; then
                continue
            else
                return 1
            fi
        fi

        if [[ "$piece" = +([1-9])*([0-9]) ]] ; then
            continue
        else
            return 1
        fi
    done

    # If we have no suffix, then we're done.
    if [[ -z "${version_array[3]}" ]] ; then
        return 0
    fi

    # Otherwise, verify that the suffix is alphanumeric+dash.
    if [[ "${version_array[3]}" = +([0-9A-Za-z-]) ]] ; then
        return 0
    else
        return 1
    fi
}

git_get_dirty_status() {
    if [[ $(git diff --shortstat 2> /dev/null | tail -n1) != "" ]] ; then
        return 1
    else
        return 0
    fi
}

git_get_number_of_untracked_files() {
    git status --porcelain 2>/dev/null| grep -c "^??"
}

git_warn_dirty() {
    local -i need_response=1
    local response

    if ! git_get_dirty_status ; then
        echo "${yellow}${bold}Warning:${normal}${yellow} Your repository has uncommitted changes. Do you really want to continue? ${bold}[Y\N]${normal}"
        need_response=0
    else
        local -i untracked_files_count
        untracked_files_count="$(git_get_number_of_untracked_files)"; readonly untracked_files_count
        if (( untracked_files_count > 0 )) ; then
            echo "${yellow}${bold}Warning:${normal}${yellow} Your repository has $untracked_files_count untracked files. Do you really want to continue? ${bold}[Y\N]${normal}"
            need_response=0
        fi
    fi

    if (( need_response == 0 )) ; then
        while true ; do
            IFS=$'\n\r' read -r -e -p '> ' response
            case "$response" in
            "Y" | "y" | "yes" | "Yes") return 0                 ;;
            "N" | "n" | "no"  | "No" ) exit 0                   ;;
            *) echo "Please respond with ${bold}[Y\N]${normal}" ;;
            esac
        done
    fi

    return 0
}

build_for_release() {
    dotnet clean --configuration Debug
    dotnet clean --configuration Release
    dotnet build --configuration Release
}

push_nuget_package() {
    local -r csproj_path="$1"
    local version_current
    local release_notes_current
    local nupkg

    package_id="$(get_package_id_from_csproj "$csproj_path")"; readonly pacakge_id
    nupkg="$(get_nupkg_filename "$csproj_path")"; readonly nupkg

    # Report current version and release notes.
    version_current="$(get_version_from_csproj "$csproj_path")"; readonly version_current
    echo "${bold}Current version:${normal} $version_current"
    release_notes_current="$(get_release_notes_from_csproj "$csproj_path")"; readonly release_notes_current
    echo "${bold}Current release notes:${normal} $release_notes_current"

    echo "Proceed to pushing \"$nupkg\"? ${bold}[Y\N]${normal}"

    while true ; do
        IFS=$'\n\r' read -r -e -p '> ' response
        case "$response" in
        "Y" | "y" | "yes" | "Yes") break                    ;;
        "N" | "n" | "no"  | "No" ) exit 0                   ;;
        *) echo "Please respond with ${bold}[Y\N]${normal}" ;;
        esac
    done

    nuget push "build/${package_id}/Release/AnyCPU/${nupkg}" -Source https://api.nuget.org/v3/index.json
}

test_csproj_path() {
    local -r csproj_path="$1"
    if [[ ! -e "$csproj_path" ]] ; then
        echo "${red}${bold}Error:${normal}${red} .csproj file could not be found at path: $csproj_path.${normal}"
        exit 2
    fi

    if [[ ! -r "$csproj_path" ]] ; then
        echo "${red}${bold}Error:${normal}${red} .csproj file is not readable.${normal}"
        exit 3
    fi

    if [[ ! -w "$csproj_path" ]] ; then
        echo "${red}${bold}Error:${normal}${red} .csproj file is not writable.${normal}"
        exit 3
    fi
}

declare bold
bold=$(tput bold); readonly bold
declare normal
normal=$(tput sgr0); readonly normal
declare red
red=$(tput setaf 1); readonly red
declare yellow
yellow=$(tput setaf 3); readonly yellow

main() {
    local -r csproj_path='src/NUnit.FixtureDependent/NUnit.FixtureDependent.csproj'
    local version_current
    local version_new
    local release_notes_current
    local release_notes_new

    # Ensure csproj exists and that we have permissions.
    test_csproj_path "$csproj_path"

    # Warn the user if the repository is dirty.
    git_warn_dirty

    # Report current version and release notes.
    version_current="$(get_version_from_csproj "$csproj_path")"; readonly version_current
    echo "${bold}Current version:${normal} $version_current"
    release_notes_current="$(get_release_notes_from_csproj "$csproj_path")"; readonly release_notes_current
    echo "${bold}Current release notes:${normal} $release_notes_current"

    # Get new version and release notes from user.
    version_new="$(get_version_new_from_user "$version_current")"; readonly version_new
    release_notes_new="$(get_new_release_notes_from_user)"; readonly release_notes_new

    # Set new version and release notes to csproj
    if [[ -n "$version_new" ]] ; then
        if ! set_version_to_csproj "$version_new" "$csproj_path" ; then
            echo "${red}${bold}Error:${normal}${red} xmlstarlet exited with non-zero code when trying to set version. Abandoning.${normal}"
            exit 1
        fi
    fi

    if [[ -n "$release_notes_new" ]] ; then
        if ! set_release_notes_to_csproj "$release_notes_new" "$csproj_path" ; then
            echo "${red}${bold}Error:${normal}${red} xmlstarlet exited with non-zero code when trying to set version. Abandoning.${normal}"
            exit 1
        fi
    fi

    build_for_release
    push_nuget_package "$csproj_path"
}

main